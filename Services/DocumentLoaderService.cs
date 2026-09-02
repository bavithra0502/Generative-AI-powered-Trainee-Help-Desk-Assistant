namespace TrainingHelpDeskApi.Services
{
    public class DocumentLoaderService : IDocumentLoaderService
    {
        private readonly string _documentsFolder;

        public DocumentLoaderService(IWebHostEnvironment env)
        {
            _documentsFolder = Path.Combine(env.ContentRootPath, "Documents");
        }

        public List<string> GetAllDocumentNames()
        {
            if (!Directory.Exists(_documentsFolder))
            {
                return [];
            }

            return Directory.GetFiles(_documentsFolder, "*.txt")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Select(name => name!)
                .OrderBy(name => name)
                .ToList();
        }

        public async Task<string> LoadDocumentAsync(string fileName)
        {
            string filePath = Path.Combine(_documentsFolder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Document not found: {filePath}");
            }

            string text = await File.ReadAllTextAsync(filePath);
            return text;
        }
    }
}
