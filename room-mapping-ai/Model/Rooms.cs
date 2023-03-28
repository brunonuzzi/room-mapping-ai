using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace room_mapping_ai.Model
{
    public class Rooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("hotelname")]
        public string HotelName { get; set; }

        [Required]
        [Column("roomname")]
        public string RoomName { get; set; }

        [Column(TypeName = "vector(1536)")]
        public string? embedding { get; set; }
    }
}
