-One way folder synchronization-

After entering 2 paths and an interval between 5 and 180 seconds (which we will call N) two folders are created.
If the first folder (main folder) is already created all its contents will be copied into the 2nd folder (copy folder) before the real-time synchronization starts
If the copy folder is already created all its contents will be deleted before the real time synchronization starts
Every second the contents of the main folder are checked for updates, if a file is added/removed/edited from the main folder it is logged into the console and into a log file.
Every N seconds folder synchronization is activated and the following happens:
  -Every file that was edited/added/removed from the main folder is edited/added/removed in the copy folder.
  -Every file that was edited/added/removed in the copy folder is retrieved to it's prior state from the main folder. 
  -All the actions are logged in the console and in the log file.
