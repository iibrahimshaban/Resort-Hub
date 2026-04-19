-- ── AMENITIES ──────────────────────────────────────────────────
INSERT INTO Amenities (Name, Description, Icon) VALUES
('Free WiFi',        'High-speed wireless internet throughout',   'fa-solid fa-wifi'),
('Swimming Pool',    'Outdoor heated pool with sun deck',         'fa-solid fa-water'),
('Air Conditioning', 'Climate control in all rooms',              'fa-solid fa-snowflake'),
('Parking',          'Private secured parking space',             'fa-solid fa-square-parking'),
('Kitchen',          'Fully equipped modern kitchen',             'fa-solid fa-utensils'),
('Sea View',         'Panoramic view of the sea',                 'fa-solid fa-umbrella-beach'),
('Gym',              'Private fitness room with equipment',       'fa-solid fa-dumbbell'),
('BBQ Area',         'Outdoor barbecue and dining area',          'fa-solid fa-fire-burner'),
('Pet Friendly',     'Pets are welcome',                          'fa-solid fa-paw'),
('Private Garden',   'Enclosed garden with outdoor furniture',    'fa-solid fa-seedling');

-- ── VILLAS ─────────────────────────────────────────────────────
INSERT INTO Villas (Name, Description, PricePerNight, Sqft, Capacity, CreatedDate, UpdatedDate, IsAvilable) VALUES
('Azure Retreat',     'A stunning beachfront villa with panoramic sea views.',        850.00,  2200, 6,  '2024-01-10', '2024-01-10', 1),
('Palm Oasis',        'Lush tropical villa surrounded by palm trees and gardens.',    620.00,  1800, 4,  '2024-01-12', '2024-01-12', 1),
('The Grand Suite',   'Luxurious villa with private pool and premium furnishings.',  1200.00,  3500, 8,  '2024-01-15', '2024-01-15', 1),
('Sunset Cove',       'Hillside villa with breathtaking sunset views every evening.', 740.00,  2000, 5,  '2024-01-18', '2024-01-18', 1),
('Coral Breeze',      'Light-filled villa steps from the beach and coral reefs.',    560.00,  1600, 4,  '2024-01-20', '2024-01-20', 1),
('The Hideaway',      'Secluded forest villa perfect for a peaceful getaway.',        480.00,  1500, 3,  '2024-02-01', '2024-02-01', 1),
('Royal Palms',       'Expansive villa with multiple terraces and a private bar.',  1050.00,  3200, 10, '2024-02-05', '2024-02-05', 1),
('Serenity Lodge',    'Minimalist design villa with zen garden and spa access.',     670.00,  1900, 4,  '2024-02-10', '2024-02-10', 1),
('Dune Villa',        'Desert-inspired villa with rooftop terrace and fire pit.',    590.00,  2100, 6,  '2024-02-14', '2024-02-14', 1),
('Marina Pearl',      'Modern villa overlooking the private marina and yacht dock.', 920.00,  2600, 7,  '2024-02-20', '2024-02-20', 1);

-- ── VILLA IMAGES ────────────────────────────────────────────────
-- one main image + one secondary per villa
INSERT INTO VillaImages (VillaId, ImageUrl, IsMain, DispalayOrder) VALUES
(1,  'https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=800', 1, 1),
(1,  'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800', 0, 2),
(2,  'https://images.unsplash.com/photo-1518780664697-55e3ad937233?w=800', 1, 1),
(2,  'https://images.unsplash.com/photo-1464983953574-0892a716854b?w=800', 0, 2),
(3,  'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800', 1, 1),
(3,  'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800', 0, 2),
(4,  'https://images.unsplash.com/photo-1499793983690-e29da59ef1c2?w=800', 1, 1),
(4,  'https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=800', 0, 2),
(5,  'https://images.unsplash.com/photo-1470770841072-f978cf4d019e?w=800', 1, 1),
(5,  'https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800', 0, 2),
(6,  'https://images.unsplash.com/photo-1465101046530-73398c7f28ca?w=800', 1, 1),
(6,  'https://images.unsplash.com/photo-1510798831971-661eb04b3739?w=800', 0, 2),
(7,  'https://images.unsplash.com/photo-1523217582562-09d0def993a6?w=800', 1, 1),
(7,  'https://images.unsplash.com/photo-1540518614846-7eded433c457?w=800', 0, 2),
(8,  'https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800', 1, 1),
(8,  'https://images.unsplash.com/photo-1439130490301-25e322d88054?w=800', 0, 2),
(9,  'https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=800', 1, 1),
(9,  'https://images.unsplash.com/photo-1533104816931-20fa691ff6ca?w=800', 0, 2),
(10, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800', 1, 1),
(10, 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800', 0, 2);

-- ── VILLA AMENITIES ─────────────────────────────────────────────
-- spread amenities across villas naturally
INSERT INTO VillaAmenities (VillaId, AmenityId) VALUES
-- Azure Retreat: WiFi, Pool, AC, Sea View, BBQ
(1,1),(1,2),(1,3),(1,6),(1,8),
-- Palm Oasis: WiFi, AC, Kitchen, Garden, Pet Friendly
(2,1),(2,3),(2,5),(2,9),(2,10),
-- The Grand Suite: all premium amenities
(3,1),(3,2),(3,3),(3,4),(3,5),(3,6),(3,7),(3,8),
-- Sunset Cove: WiFi, AC, Sea View, BBQ, Garden
(4,1),(4,3),(4,6),(4,8),(4,10),
-- Coral Breeze: WiFi, Pool, AC, Kitchen
(5,1),(5,2),(5,3),(5,5),
-- The Hideaway: WiFi, AC, Kitchen, Pet Friendly, Garden
(6,1),(6,3),(6,5),(6,9),(6,10),
-- Royal Palms: WiFi, Pool, AC, Parking, Kitchen, Sea View, Gym, BBQ
(7,1),(7,2),(7,3),(7,4),(7,5),(7,6),(7,7),(7,8),
-- Serenity Lodge: WiFi, AC, Kitchen, Gym, Garden
(8,1),(8,3),(8,5),(8,7),(8,10),
-- Dune Villa: WiFi, AC, Parking, BBQ, Garden
(9,1),(9,3),(9,4),(9,8),(9,10),
-- Marina Pearl: WiFi, Pool, AC, Parking, Sea View, Gym
(10,1),(10,2),(10,3),(10,4),(10,6),(10,7);

-- ── BOOKINGS (5 bookings by the single user, non-overlapping) ───
-- Status: 0 = Pending, 1 = Confirmed, 2 = Cancelled, 3 = Completed
INSERT INTO Bookings (BookingDate, CheckInDate, CheckOutDate, TotalCost, Status, VillaId, UserId) VALUES
-- Villa 1 — Azure Retreat, 5 nights, completed
('2024-03-01', '2024-03-10', '2024-03-15',  4250.00, 3, 1,  '75172400-fe60-4873-b077-8552d697a4b6'),
-- Villa 3 — The Grand Suite, 3 nights, completed
('2024-04-05', '2024-04-20', '2024-04-23',  3600.00, 3, 3,  '75172400-fe60-4873-b077-8552d697a4b6'),
-- Villa 5 — Coral Breeze, 7 nights, confirmed
('2024-06-10', '2024-07-01', '2024-07-08',  3920.00, 1, 5,  '75172400-fe60-4873-b077-8552d697a4b6'),
-- Villa 7 — Royal Palms, 4 nights, confirmed
('2024-08-01', '2024-08-15', '2024-08-19',  4200.00, 1, 7,  '75172400-fe60-4873-b077-8552d697a4b6'),
-- Villa 9 — Dune Villa, 6 nights, pending
('2024-10-20', '2024-11-05', '2024-11-11',  3540.00, 0, 9,  '75172400-fe60-4873-b077-8552d697a4b6');