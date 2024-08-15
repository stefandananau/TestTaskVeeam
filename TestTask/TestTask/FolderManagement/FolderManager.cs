using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestTask.Logging;

namespace TestTask.FolderManagement
{
    internal class FolderManager
    {
        private const int CHECK_INTERVAL = 1;
        private string _mainFolder;
        private string _copyFolder;
        private int _syncInterval;
        private int _syncNumber;
        private bool _updateInstantly = false;
        private bool _isStarted = false;

        public FolderManager(string mainFolder, string copyFolder, int syncInterval)
        {
            _mainFolder = mainFolder;
            _copyFolder = copyFolder;
            _syncInterval = syncInterval;
            _syncNumber = 1;
            CreateMainFolder();
            CreateCopyFolder();
        }

        public void Start()
        {
            if (_isStarted) return;
            _isStarted = true;

        }

        private void CreateMainFolder()
        {
            var folder = _mainFolder;
            if (Directory.Exists(folder))
            {
                Logger.Log("Main folder already exists, contents will be copied into copy folder", LogLevel.Info);
                _updateInstantly = true;
            }
            else
            {
                Directory.CreateDirectory(folder);
                Logger.Log($"Main folder was created at '{folder}' path", LogLevel.Info);
            }
        }

        private void CreateCopyFolder()
        {
            var folder = _copyFolder;
            if (Directory.Exists(folder))
            {
                Logger.Log("Copy folder already exists, all contents will be removed", LogLevel.Warning);
                Directory.Delete(folder, true);
            }
            Logger.Log($"Copy folder was created at '{folder}' path", LogLevel.Info);
            Directory.CreateDirectory(folder);
            if (_updateInstantly)
            {
                Logger.Log($"Copying all initial files from '{_mainFolder}' to '{_copyFolder}'", LogLevel.Info);
                Synchronize();
            }
        }

        private void Synchronize()
        {
            Logger.Log($"Synchronization with ID='{_syncNumber}' has begun", LogLevel.Info);

            List<FolderContentItem> mainFolderContents = GetDirectoryContent(new DirectoryInfo(_mainFolder)).OrderByDescending(item => item.Depth).ToList();
            List<FolderContentItem> copyFolderContents = GetDirectoryContent(new DirectoryInfo(_copyFolder)).OrderByDescending(item => item.Depth).ToList();
            
            RemoveExtraItemsFromCopyFolder(mainFolderContents, copyFolderContents);
            AddItemsToCopyFolder(mainFolderContents, copyFolderContents);
            
            Logger.Log($"Synchronization with ID='{_syncNumber}' is finished", LogLevel.Info);
        }

        private void RemoveExtraItemsFromCopyFolder(List<FolderContentItem> mainFolderContents, List<FolderContentItem> copyFolderContents)
        {
            mainFolderContents.ForEach(item => item.Name = item.Name.Replace(_mainFolder, _copyFolder));
            List<FolderContentItem> itemsToRemove = copyFolderContents.Where(copyItem => !mainFolderContents.Any(mainItem => copyItem.Equals(mainItem))).ToList();
            itemsToRemove.ForEach(item => RemoveFromCopyFolder(item));
        }

        private void AddItemsToCopyFolder(List<FolderContentItem> mainFolderContents, List<FolderContentItem> copyFolderContents)
        {
            mainFolderContents.Reverse();
            copyFolderContents.Reverse();
            List<FolderContentItem> itemsToAdd = mainFolderContents.Where(mainItem => !copyFolderContents.Any(copyItem => mainItem.Equals(copyItem))).ToList();
            itemsToAdd.ForEach(item => AddToCopyFolder(item));
        }

        private void RemoveFromCopyFolder(FolderContentItem item)
        {
            try
            {
                switch (item.ContentType)
                {
                    case FolderContentType.Folder:
                        Directory.Delete(item.Name, true);
                        Logger.Log($"Folder '{item.Name}' succesfully removed from copy folder", LogLevel.Info);
                        break;
                    case FolderContentType.File:
                        File.Delete(item.Name);
                        Logger.Log($"File '{item.Name}' succesfully removed from copy folder", LogLevel.Info);
                        break;
                    default:
                        Logger.Log($"Item '{item.Name}' has no type", LogLevel.Error);
                        break;
                }
            }
            catch (Exception)
            {
                Logger.Log($"Error on removing file '{item.Name}' from the copy folder", LogLevel.Error);
            }
        }

        private void AddToCopyFolder(FolderContentItem item)
        {
            try
            {
                switch (item.ContentType)
                {
                    case FolderContentType.Folder:
                        Directory.CreateDirectory(item.Name);
                        Logger.Log($"Folder '{item.Name}' succesfully added to copy folder", LogLevel.Info);
                        break;
                    case FolderContentType.File:
                        File.Copy(item.Name.Replace(_copyFolder, _mainFolder), item.Name);
                        Logger.Log($"File '{item.Name}' succesfully added to copy folder", LogLevel.Info);
                        break;
                    default:
                        Logger.Log($"Item '{item.Name}' has no type", LogLevel.Error);
                        break;
                }
            }
            catch (Exception)
            {
                Logger.Log($"Error on copying file '{item.Name}' to the copy folder", LogLevel.Error);
            }
        }

        private IEnumerable<FolderContentItem> GetDirectoryContent(DirectoryInfo directory, int depth = 0)
        {
            if (depth != 0)
            {
                yield return new FolderContentItem(directory.FullName, depth, FolderContentType.Folder);
            }
            foreach (var subdirectory in directory.GetDirectories())
                foreach (var item in GetDirectoryContent(subdirectory, depth + 1))
                    yield return item;

            foreach (var file in directory.GetFiles())
                yield return new FolderContentItem(file.FullName, depth + 1, FolderContentType.File);
        }
    }
}
