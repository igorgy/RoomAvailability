
# RoomAvailability test project.

A program to manage hotel room availability and reservations. 


## Usage
Run console application providing a pth to input Hotels and Bookings files:
```
RoomAvailability.exe --bookings c:\projects\RoomAvailability\TestData\bookings.json 
 --hotels c:\projects\RoomAvailability\TestData\hotels.json 
```
In case Hotels and Bookings files are placed in the same folder executable is, run command can be simplified to:
```
RoomAvailability.exe --bookings bookings.json --hotels hotels.json 
```
## Commands
**Availability** Command, gives the availability count for the specified room type and
 date range.
 ```
Availability(H1, 20240901, SGL)  
Availability(H1, 20240901-20240903, DBL)  
 ```
**Search** Command, returns a comma separated list of date ranges and availability where the room is available.
 ```
Search(H1, 365, SGL)  
 ```