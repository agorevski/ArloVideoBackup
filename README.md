# ArloVideoBackup
Small tool that downloads your arlo videos so you don't have to pay the monthly fee.

Usage:  BackupVideos.exe --o "C:\saveFolder" --d 7 --u myusername@gmail.com --p mypassword

*  o - The base output directory to save the arlo files
*  d - The number of days (including today) that you'd like to go back and scrape from Arlo's services
*  u - Your email address
*  p - Your password

You can run this tool serially indefinitely - it will find (and skip) pre-downloaded files.