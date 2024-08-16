-One way folder synchronization-


If all the instructions provided at the start of the console application are respected the application will

->read a log path, main folder path, copy folder path and a synchronization interval from the console

->log any modification made to the main folder every second

->copy and log modifications to the main folder into the copy folder every N (synchronization interval seconds)


Some error handling mechanisms are in place but in order to have the best experience follow the instructions and warnings (which are added here and at the start of the console application)

->Visual Studio should be opened with administrator rights

->Paths should be of format 'Disk:\folder1\folder2\etc..'

->If the main folder already exists, all existing items within it will be copied to the copy folder (automatically)

->If the copy folder already exists, all existing items within it will be DELETED (automatically)

->If the synchronization interval is not between 5 and 180 seconds or is invalid it will be defaulted to 30 seconds

->The checking interval (it checks for added files in the main directory without copying them to the copy directory) is 1 second by default