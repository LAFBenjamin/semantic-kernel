Meeting Room Booking Project using an LLM with semantic kernel 

This project is a C# console application that utilizes a language model (LLM) to facilitate booking meeting rooms within a company. The application guides users through gathering the necessary information for booking a room, checks room availability, and confirms the booking.

https://learn.microsoft.com/en-us/semantic-kernel/overview/

Features

Meeting information gathering: Collects details such as the date, start time, end time, number of participants, and location.
Time slot management: Ensures the validity of the requested booking time (e.g., no bookings are allowed for past dates).
Smart booking: If no time is provided, the search focuses on the date only, allowing for flexible time slots.
Availability checking: The system verifies room availability using the RoomAvailabilities field.
Booking recap and confirmation: The user must confirm the booking after all details have been provided.
Error handling: If no rooms are available, the user is prompted to search for another date.

Prerequisites

.NET SDK 6.0 or higher
OpenAI API Key (for GPT-3.5-turbo)
