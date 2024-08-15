namespace TestTask.FolderManagement
{
    internal class FolderContentItem
    {
        public string Name;
        public int Depth;
        public FolderContentType ContentType;


        public FolderContentItem(string name, int depth, FolderContentType contentType)
        {
            Name = name;
            Depth = depth;
            ContentType = contentType;
        }

        public bool Equals(FolderContentItem other) => Name.Equals(other.Name) && Depth.Equals(other.Depth) && ContentType.Equals(other.ContentType);
        
    }
}
