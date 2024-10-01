using System.ComponentModel;
using Microsoft.SemanticKernel;

class BookingPlugin {
    
    [KernelFunction("get_booking_room_available")]
    [Description("Checking if a room is available for booking")]    
    public async Task<string> GetBookingRoomAvailable(){
        return "salle 1";
    }
}