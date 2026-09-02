using TrainingHelpDeskApi.Models;
using TrainingHelpDeskApi.Models.Dtos;

namespace TrainingHelpDeskApi.Services
{
    public class RagService : IRagService
    {
        private const int TopK = 3;

        // Phrase the chat model is instructed to use when the knowledge base
        // has no relevant information, so we can reliably detect "not found" answers.
        private const string NotFoundPhrase = "not available in the trainee knowledge base";

        private readonly IDocumentLoaderService _documentLoader;
        private readonly IChunkingService _chunkingService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IRetrievalService _retrievalService;
        private readonly IChatService _chatService;
        private readonly KnowledgeBaseService _knowledgeBaseService;

        public RagService(
            IDocumentLoaderService documentLoader,
            IChunkingService chunkingService,
            IEmbeddingService embeddingService,
            IRetrievalService retrievalService,
            IChatService chatService,
            KnowledgeBaseService knowledgeBaseService)
        {
            _documentLoader = documentLoader;
            _chunkingService = chunkingService;
            _embeddingService = embeddingService;
            _retrievalService = retrievalService;
            _chatService = chatService;
            _knowledgeBaseService = knowledgeBaseService;
        }

        public async Task<int> BuildKnowledgeBaseAsync()
        {
            List<string> fileNames = _documentLoader.GetAllDocumentNames();

            var allChunks = new List<EmbeddingDocument>();

            foreach (string fileName in fileNames)
            {
                string text = await _documentLoader.LoadDocumentAsync(fileName);
                List<EmbeddingDocument> chunks = _chunkingService.CreateChunks(text, fileName);
                allChunks.AddRange(chunks);
            }

            List<EmbeddingDocument> embeddedChunks = await _embeddingService.GenerateEmbeddingAsync(allChunks);

            _knowledgeBaseService.SetDocuments(embeddedChunks);

            return embeddedChunks.Count;
        }

        public async Task<AskResponseDto> AskAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty");
            }

            List<EmbeddingDocument> documents = _knowledgeBaseService.GetDocuments();

            if (documents.Count == 0)
            {
                // Knowledge base has not been built yet in this session; build it on demand.
                await BuildKnowledgeBaseAsync();
                documents = _knowledgeBaseService.GetDocuments();
            }

            float[] queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);

            List<RetrieveDocument> retrievedDocuments =
                _retrievalService.Search(queryEmbedding, documents, TopK);

            // Only keep chunks that are meaningfully similar to the question.
            List<RetrieveDocument> relevantDocuments = retrievedDocuments
                .Where(x => x.Similarity >= RetrievalService.RelevanceThreshold)
                .ToList();

            if (relevantDocuments.Count == 0)
            {
                return new AskResponseDto
                {
                    Question = question,
                    Answer = "I'm sorry, that information is not available in the trainee knowledge base. " +
                             "Please reach out to your trainer or training coordinator for further assistance.",
                    Sources = [],
                    AnswerFoundInKnowledgeBase = false
                };
            }

            string context = string.Join("\n\n", relevantDocuments.Select(x =>
                $"Source: {x.Document.Source}\nContent: {x.Document.Text}"));

            string systemPrompt = $"""
                You are the AI-Powered Trainee Help Desk Assistant for a training organization.
                Answer the trainee's question using only the information provided in the context below.
                If the answer cannot be found in the context, clearly say that the information is
                {NotFoundPhrase} and suggest the trainee contact their trainer or training coordinator.
                Do not invent or assume any training policy that is not stated in the context.
                Keep the answer clear, concise, and friendly.
                """;

            string userPrompt = $"""
                CONTEXT
                {context}

                TRAINEE QUESTION
                {question}

                Answer the trainee's question using only the context provided above.
                """;

            string answer = await _chatService.GenerateAnswerAsync(systemPrompt, userPrompt);

            bool foundInKnowledgeBase = !answer.Contains(NotFoundPhrase, StringComparison.OrdinalIgnoreCase);

            return new AskResponseDto
            {
                Question = question,
                Answer = answer,
                Sources = relevantDocuments.Select(x => x.Document.Source).Distinct().ToList(),
                AnswerFoundInKnowledgeBase = foundInKnowledgeBase
            };
        }
    }
}
