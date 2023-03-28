using System.ComponentModel.DataAnnotations.Schema;

namespace room_mapping_ai.Model
{
    public class RoomsResult
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("hotelname")]
        public string HotelName { get; set; }

        [Column("roomname")]
        public string RoomName { get; set; }

        [Column("vector_distance")]
        public double? VectorDistance { get; set; }
    }
}
