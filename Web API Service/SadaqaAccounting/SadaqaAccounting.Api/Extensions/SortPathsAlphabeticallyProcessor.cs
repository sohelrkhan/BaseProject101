namespace SadaqaAccounting.Api.Extensions
{
    public class SortPathsAlphabeticallyProcessor : IDocumentProcessor
    {
        public void Process(DocumentProcessorContext context)
        {
            // Get the sorted key-value pairs
            var sortedPaths = context.Document.Paths.OrderBy(p => p.Key).ToList();

            // Clear the original paths collection
            context.Document.Paths.Clear();

            // Re-add in sorted order
            foreach (var (key, value) in sortedPaths)
            {
                context.Document.Paths.Add(key, value);
            }
        }
    }
}