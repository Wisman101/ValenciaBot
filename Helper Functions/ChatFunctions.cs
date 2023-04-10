// using Microsoft.Devices.Geolocation;
// using System;

// public static void Main()
// {
//     // Define the set of locations to compare against
//     var locations = new[]
//     {
//         new GeolocationCoordinate(47.608013, -122.335167), // Seattle
//         new GeolocationCoordinate(51.507222, -0.127758),   // London
//         new GeolocationCoordinate(40.712776, -74.005974),  // New York
//         new GeolocationCoordinate(-33.865143, 151.209900)  // Sydney
//     };

//     // Define the reference location
//     var referenceLocation = new GeolocationCoordinate(37.774929, -122.419416); // San Francisco

//     // Find the nearest location
//     GeolocationCoordinate nearestLocation = null;
//     double nearestDistance = double.MaxValue;

//     foreach (var location in locations)
//     {
//         var distance = referenceLocation.GetDistanceTo(location);

//         if (distance < nearestDistance)
//         {
//             nearestLocation = location;
//             nearestDistance = distance;
//         }
//     }

//     // Print the result
//     Console.WriteLine($"The nearest location is ({nearestLocation.Latitude}, {nearestLocation.Longitude}) at a distance of {nearestDistance} meters.");
// }
