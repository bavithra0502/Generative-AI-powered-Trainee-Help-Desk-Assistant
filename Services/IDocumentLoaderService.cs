namespace TrainingHelpDeskApi.Services
{
    public interface IDocumentLoaderService
    {
        // Loads a single document's text content by file name.
        Task<string> LoadDocumentAsync(string fileName);

        // Returns the file names of every document available in the Trainee Knowledge Base.
        List<string> GetAllDocumentNames();
    }
}
