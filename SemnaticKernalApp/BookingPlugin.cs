using System.ComponentModel;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Microsoft.SemanticKernel;

class BookingPlugin
{

    List<Room> rooms = new List<Room>
        {
            new Room { Name = "Conference Room A", Capacity = 50, Location = "1st Floor", Description = "Spacious room with modern AV equipment.", Image = "conference_a.jpg", 
            RoomAvailabilities ={new RoomAvailability(
                DateTime.Now.Date,new List<Slot>(){
                new Slot(new TimeSpan(14, 30, 0), new TimeSpan(18, 45, 0)),
                new Slot(new TimeSpan(9, 30, 0), new TimeSpan(11, 0, 0))
                })
                ,
                new RoomAvailability(
                DateTime.Now.Date.AddDays(1),new List<Slot>(){
                new Slot(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
                new Slot(new TimeSpan(18, 0, 0), new TimeSpan(20, 0, 0))
                }),
                new RoomAvailability(
                DateTime.Now.Date.AddDays(2),new List<Slot>(){
                new Slot(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
                new Slot(new TimeSpan(18, 0, 0), new TimeSpan(20, 0, 0))
                })},
            },
            new Room { Name = "Meeting Room B", Capacity = 12, Location = "2nd Floor", Description = "Cozy room for small team meetings.", Image = "meeting_b.jpg", 
            RoomAvailabilities ={new RoomAvailability(
                DateTime.Now.Date,new List<Slot>(){
                new Slot(new TimeSpan(11, 30, 0), new TimeSpan(13, 0, 0))
                }),
                new RoomAvailability(
                DateTime.Now.Date.AddDays(3),new List<Slot>(){
                new Slot(new TimeSpan(11, 0, 0), new TimeSpan(15, 0, 0)),
                new Slot(new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0))
                })},
            },
            new Room { Name = "Executive Suite", Capacity = 5, Location = "3rd Floor", Description = "Private executive suite with premium amenities.", Image = "executive_suite.jpg" },
            new Room { Name = "Training Room", Capacity = 30, Location = "Ground Floor", Description = "Equipped for training sessions with whiteboards and projectors.", Image = "training_room.jpg" },
            new Room { Name = "Workshop Room", Capacity = 40, Location = "4th Floor", Description = "Perfect for workshops and interactive sessions.", Image = "workshop_room.jpg" },
            new Room { Name = "Boardroom", Capacity = 20, Location = "5th Floor", Description = "Formal setting with a large table for board meetings.", Image = "boardroom.jpg" },
            new Room { Name = "Lecture Hall", Capacity = 100, Location = "Ground Floor", Description = "Large hall for presentations and lectures.", Image = "lecture_hall.jpg" },
            new Room { Name = "Creative Space", Capacity = 15, Location = "3rd Floor", Description = "Bright and inspiring room for brainstorming sessions.", Image = "creative_space.jpg" },
            new Room { Name = "Breakout Room", Capacity = 8, Location = "2nd Floor", Description = "Ideal for quick discussions and breakouts.", Image = "breakout_room.jpg" },
            new Room { Name = "Quiet Room", Capacity = 5, Location = "1st Floor", Description = "Small, quiet room for focused work.", Image = "quiet_room.jpg" },
            new Room { Name = "Event Hall", Capacity = 200, Location = "Main Hall", Description = "Spacious venue for large events and conferences.", Image = "event_hall.jpg" },
            new Room { Name = "Tech Lab", Capacity = 25, Location = "Lower Ground", Description = "Advanced room for technical workshops with high-end equipment.", Image = "tech_lab.jpg" },
            new Room { Name = "Audio Studio", Capacity = 10, Location = "3rd Floor", Description = "Designed for audio recording and production.", Image = "audio_studio.jpg" },
            new Room { Name = "Art Room", Capacity = 15, Location = "4th Floor", Description = "Creative space for art classes and activities.", Image = "art_room.jpg" },
            new Room { Name = "Meditation Room", Capacity = 10, Location = "5th Floor", Description = "Calm and serene space for meditation and relaxation.", Image = "meditation_room.jpg" }
        };

   
    public BookingPlugin()
    {
        
    }


    [KernelFunction("book_room")]
    [Description("Book a meeting room")]
    public async Task<string> BookRoom(
        [Description("id room to book")] string? idRoom,
        [Description("Meeting of date (dd/MM/yyyy)")] string? dateOfMeetingStr,
        [Description("Start time in hour)")] string startTimeStr ,
        [Description("End time in hour")] string endTimeStr)
    {
       return  "reservation ok ";
    }

    [KernelFunction("get_booking_room_available")]
    [Description("Checking if a room is available for booking or list room available")]
    public async Task<string> GetBookingRoomAvailable(
        [Description("Number of participants")] int? numberParticipants,
        [Description("Meeting of date (dd/MM/yyyy)")] string? dateOfMeetingStr,
        [Description("Start time in hour)")] string startTimeStr ,
        [Description("End time in hour")] string endTimeStr )
    {
        List<Room> roomAvailable = rooms.ToList();
        
         DateTime? dateOfMeeting = null;
         if (DateTime.TryParseExact(dateOfMeetingStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate))
        {
            dateOfMeeting = parsedStartDate;
        }
        TimeSpan startTime;
        TimeSpan.TryParse(startTimeStr,out startTime);

        TimeSpan endTime;
        TimeSpan.TryParse(endTimeStr,out endTime);

        if(dateOfMeeting is not null && startTime != new TimeSpan(0, 0, 0) && endTime != new TimeSpan(0, 0, 0))
            roomAvailable.RemoveAll(room =>  !room.RoomAvailabilities.Any(a=> a.Days == dateOfMeeting && a.Slots.Any(a=> a.Start<= startTime && a.End >= endTime)));
        else if(dateOfMeeting is not null && startTime != new TimeSpan(0, 0, 0) && endTime == new TimeSpan(0, 0, 0))
            roomAvailable.RemoveAll(room =>  !room.RoomAvailabilities.Any(a=> a.Days == dateOfMeeting && a.Slots.Any(a=> a.Start<= startTime && a.End > startTime)));
        else
            roomAvailable.RemoveAll(room =>  !room.RoomAvailabilities.Any(a=> a.Days == dateOfMeeting));
           
        // Filtrer par nombre de participants si fourni
        if (numberParticipants is not null)
            roomAvailable.RemoveAll(room => room.Capacity <= numberParticipants);
        
       

        return FormatRoomsForDisplay(roomAvailable,dateOfMeeting,startTime,endTime);
    }
   
  public string FormatRoomsForDisplay(List<Room> rooms, DateTime? dateOfMeeting, TimeSpan start, TimeSpan end)
{
    if( rooms.Count == 0) return "No rooms available for the specified criteria.";

    var sb = new StringBuilder();
    sb.AppendLine("### List of Available Rooms:\n");

    foreach (var room in rooms)
    {
        AppendRoomDetails(sb, room);
        AppendAvailability(sb, room, dateOfMeeting, start, end);
    }

    return sb.ToString();
}

    private void AppendRoomDetails(StringBuilder sb, Room room)
    {
        sb.AppendLine($"**Room Name**: {room.Name}");
        sb.AppendLine($"**Capacity**: {room.Capacity}");
        sb.AppendLine($"**Location**: {room.Location}");
    }

    private void AppendAvailability(StringBuilder sb, Room room, DateTime? dateOfMeeting, TimeSpan start, TimeSpan end)
    {
        sb.AppendLine("**Availability slot**:");
        sb.AppendLine($"**Date of meetings : {dateOfMeeting?.ToString("yyyy-MM-dd")}**:");

        var roomAvailability = room.RoomAvailabilities
            .FirstOrDefault(availability => availability.Days == dateOfMeeting);
        
        if (roomAvailability != null)
        {
            foreach (var slot in roomAvailability.Slots)
            {
                var availabilityInfo = GetAvailabilityString(slot, start, end);
                if (!string.IsNullOrEmpty(availabilityInfo))
                {
                    sb.AppendLine(availabilityInfo);
                }
            }
        }
        sb.AppendLine();
    }

    private string GetAvailabilityString(Slot slot, TimeSpan start, TimeSpan end)
{
    if (start != TimeSpan.Zero && end != TimeSpan.Zero)
    {
        if (slot.Start <= start && slot.End > end)
        {
            return $"  Time: {start} - {slot.End}";
        }
    }
    else if (start != TimeSpan.Zero)
    {
        if (slot.Start <= start && slot.End > start)
        {
            return $"  Time: {start} - free room until {slot.End}";
        }
    }
    
    return $"  Time: {slot.Start} - {slot.End}";
}
}

class Room
{
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }

    public List<RoomAvailability> RoomAvailabilities {get; set;} = new();
}

class RoomAvailability
{
    public DateTime Days;
    public List<Slot> Slots;

    public RoomAvailability(DateTime days, List<Slot> slotAvailability )
    {
        Days = days;
        Slots = slotAvailability;
    }
} 

public record Slot(TimeSpan Start, TimeSpan End);
