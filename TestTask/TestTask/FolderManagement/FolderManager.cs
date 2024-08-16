using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private int _count;
        private List<FolderContentItem> oldMainFolderContents;

        public FolderManager(string mainFolder, string copyFolder, int syncInterval)
        {
            _mainFolder = mainFolder;
            _copyFolder = copyFolder;
            _syncInterval = syncInterval;
            _syncNumber = 1;
            _count = 1;
            CreateMainFolder();
            CreateCopyFolder();
        }

        public void Start()
        {
            if (_isStarted) return;
            _isStarted = true;
            oldMainFolderContents = GetDirectoryContent(new DirectoryInfo(_mainFolder)).OrderBy(item => item.Depth).ToList();
            Logger.Log("The real-time synchronization and loggging is started", LogLevel.Info);
            new Timer(Tick, new AutoResetEvent(false), CHECK_INTERVAL * 1000, CHECK_INTERVAL * 1000);
        }

        public void Tick(Object stateiIfo)
        {
            LogMainFolderUpdates();
            if(_count % _syncInterval == 0)
            {
                Synchronize();
            }
            _count++;
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

        private void LogMainFolderUpdates()
        {
            List<FolderContentItem> newMainFolderContents = GetDirectoryContent(new DirectoryInfo(_mainFolder)).OrderBy(item => item.Depth).ToList();
            List<FolderContentItem> removedItems = oldMainFolderContents.Where(oldItem => !newMainFolderContents.Any(newItem => oldItem.SurfaceEquals(newItem))).ToList();
            List<FolderContentItem> addedItems = newMainFolderContents.Where(newItem => !oldMainFolderContents.Any(oldItem => newItem.SurfaceEquals(oldItem))).ToList();
            removedItems.OrderByDescending(item => item.Depth).ToList().ForEach(item => LogMainFolderModifications(item, "removed from"));
            addedItems.ForEach(item => LogMainFolderModifications(item, "added to"));
            GetModifiedItems(newMainFolderContents, oldMainFolderContents).ForEach(item => LogMainFolderModifications(item, "modified in"));
            oldMainFolderContents = newMainFolderContents;
        }

        private void LogMainFolderModifications(FolderContentItem item, string logActionText)
        {
            switch (item.ContentType)
            {
                case FolderContentType.Folder:
                    Logger.Log($"Folder '{item.Name}' was {logActionText} main folder", LogLevel.Info);
                    break;
                case FolderContentType.File:
                    Logger.Log($"File '{item.Name}' was {logActionText} main folder", LogLevel.Info);
                    break;
                default:
                    Logger.Log($"Item '{item.Name}' has no type", LogLevel.Error);
                    break;
            }
        }

        private void Synchronize()
        {
            Logger.Log($"Synchronization with ID='{_syncNumber}' has begun", LogLevel.Info);

            List<FolderContentItem> mainFolderContents = GetDirectoryContent(new DirectoryInfo(_mainFolder)).OrderByDescending(item => item.Depth).ToList();
            List<FolderContentItem> copyFolderContents = GetDirectoryContent(new DirectoryInfo(_copyFolder)).OrderByDescending(item => item.Depth).ToList();
            RemoveExtraItemsFromCopyFolder(mainFolderContents, copyFolderContents);
            AddItemsToCopyFolder(mainFolderContents, copyFolderContents);
            GetModifiedItems(copyFolderContents, mainFolderContents).ForEach(item => ReplaceInCopyFolder(item));
            
            Logger.Log($"Synchronization with ID='{_syncNumber}' is finished", LogLevel.Info);
            _syncNumber++;
        }

        private void RemoveExtraItemsFromCopyFolder(List<FolderContentItem> mainFolderContents, List<FolderContentItem> copyFolderContents)
        {
            mainFolderContents.ForEach(item => item.Name = item.Name.Replace(_mainFolder, _copyFolder));
            List<FolderContentItem> itemsToRemove = copyFolderContents.Where(copyItem => !mainFolderContents.Any(mainItem => copyItem.SurfaceEquals(mainItem))).ToList();
            itemsToRemove.ForEach(item => RemoveFromCopyFolder(item));
        }

        private void AddItemsToCopyFolder(List<FolderContentItem> mainFolderContents, List<FolderContentItem> copyFolderContents)
        {
            mainFolderContents.Reverse();
            copyFolderContents.Reverse();
            List<FolderContentItem> itemsToAdd = mainFolderContents.Where(mainItem => !copyFolderContents.Any(copyItem => mainItem.SurfaceEquals(copyItem))).ToList();
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

        private void ReplaceInCopyFolder(FolderContentItem item)
        {
            //This method is only used for files
            try
            {
                File.Delete(item.Name);
                File.Copy(item.Name.Replace(_copyFolder, _mainFolder), item.Name);
                Logger.Log($"File '{item.Name}' succesfully replaced in copy folder", LogLevel.Info);
            }
            catch(Exception)
            {
                Logger.Log($"Error on replacing file '{item.Name}' in the copy folder, the file should not be open and with saved modifications at the moment of synchronization", LogLevel.Error);
            }
        }

        private List<FolderContentItem> GetModifiedItems(List<FolderContentItem> firstSet, List<FolderContentItem> secondSet) => firstSet
            .Where(item => item.ContentType == FolderContentType.File)
            .Where(firstItem => secondSet.Any(secondItem => firstItem.SurfaceEquals(secondItem) && firstItem.LastModificationDateTime != secondItem.LastModificationDateTime))
            .ToList();

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
                yield return new FolderContentItem(file.FullName, depth + 1, FolderContentType.File, file.LastWriteTime);
        }
    }
}
