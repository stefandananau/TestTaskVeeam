using System;

namespace TestTask.FolderManagement
{
    internal class FolderContentItem
    {
        public string Name;
        public int Depth;
        public FolderContentType ContentType;
        public DateTime LastModificationDateTime;

        public FolderContentItem(string name, int depth, FolderContentType contentType, DateTime lastModificationTime = new DateTime())
        {
            Name = name;
            Depth = depth;
            ContentType = contentType;
            LastModificationDateTime = lastModificationTime;
        }

        public bool SurfaceEquals(FolderContentItem other) => Name.Equals(other.Name) && Depth.Equals(other.Depth) && ContentType.Equals(other.ContentType);
    }
}
