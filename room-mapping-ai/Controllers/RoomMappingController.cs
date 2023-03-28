using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI_API;
using Pgvector;
using room_mapping_ai.Model;

namespace room_mapping_ai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomMappingController : ControllerBase
    {

        private readonly RoomMappingContext _context;
        private readonly OpenAIAPI _openAIAPI;

        public RoomMappingController(RoomMappingContext context, IOptions<OpenAIConfiguration> openAIConfig)
        {
            _context = context;
            if (string.IsNullOrWhiteSpace(openAIConfig.Value.ApiKey))
            {
                throw new ArgumentException("The OpenAI API key must be provided in the appsettings.json file. You can generate you api key here: https://platform.openai.com/account/api-keys");
            }
            _openAIAPI = new OpenAIAPI(openAIConfig.Value.ApiKey);
        }

        [HttpPost("GetMostSimilarRooms")]
        public async Task<ActionResult<Rooms>> GetMostSimilarRooms(string roomName)
        {
            Vector? embedding;
            Rooms? existingRoom = await FindRoomByName(roomName);

            List<RoomsResult> searchResults;
            if (existingRoom is not null)
            {
                embedding = new Vector(existingRoom.embedding);
                searchResults = await CalculateDistances(embedding);
            }
            else
            {
                var result = await _openAIAPI.Embeddings.CreateEmbeddingAsync(roomName);
                embedding = new Vector(result.Data[0].Embedding);

                searchResults = await CalculateDistances(embedding);
                SaveNewVector(roomName, embedding);
            }

            return Ok(searchResults);
        }

        private async Task<Rooms> FindRoomByName(string roomName) =>
            (await _context.Rooms.FromSql($"SELECT Id,HotelName,RoomName,embedding::text FROM rooms where RoomName = {roomName}").ToListAsync()).FirstOrDefault();

        private void SaveNewVector(string roomName, Vector? embedding) =>
            _context.Database.ExecuteSql($"INSERT INTO rooms (HotelName,RoomName,embedding) VALUES ('Mallorca Rocks',{roomName},{embedding.ToString()}::vector)");

        private async Task<List<RoomsResult>> CalculateDistances(Vector? embedding) =>
            await _context.RoomsResult.FromSql($"select id,HotelName,RoomName,embedding <=> {embedding.ToString()}::vector as vector_distance from rooms ORDER BY vector_distance limit 5").ToListAsync();
    }
}