using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Pages.Shared;

namespace PlantsInformationWeb.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IPlantRepository _plantRepository;

        public AIService(IConfiguration configuration, IPlantRepository plantRepository)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"];
            _plantRepository = plantRepository;
        }

        public async Task<string> AskAIAsync(List<MessageDto> messages, string userMessage)
        {
            // 🌿 Lấy dữ liệu nội bộ
            var context = await BuildPlantContextForAIAsync(userMessage);
            Console.WriteLine("===== Dữ liệu thông tin cây được lấy từ DB ===== ");
            Console.WriteLine(context);

            // 🌱 Prompt động
            var systemPrompt = $@"
        Bạn là Plant Assistant 🌱 – trợ lý AI thân thiện, hiểu biết sâu rộng về cây trồng, sinh học, khí hậu và chăm sóc thực vật.

        Dưới đây là dữ liệu nội bộ của hệ thống PlantsInformationWeb (chỉ mang tính tham khảo, không giới hạn):
        {context}

        Hướng dẫn:
        - Ưu tiên dựa trên thông tin trong dữ liệu nội bộ để trả lời, tuy nhiên hãy trả lời tự nhiên nhất có thể – không quá gò bó trong dữ liệu được trả về.
        - Nếu dữ liệu nội bộ thiếu thông tin, bạn có thể bổ sung kiến thức từ hiểu biết của mình.
        - Luôn trả lời thân thiện, tự nhiên, gần gũi với người dùng (không khô khan).
        - Trả lời ngắn gọn, dễ hiểu, chỉ dùng văn bản thuần túy – không bảng.
        - Nếu người dùng chỉ hỏi xác nhận hoặc hỏi chung chung về một loại cây (ví dụ: “Bạn biết hoa hướng dương chứ?”, “Cây này là gì?”), vui lòng trả lời ngắn gọn, xác nhận hoặc mô tả đơn giản.
        - Nếu người dùng hỏi chi tiết (ví dụ: về cách trồng, bệnh thường gặp, chu kỳ sinh trưởng…), hãy trả lời đầy đủ và có thể trích dẫn thông tin liên quan.
        - Nếu người dùng hỏi ngoài chủ đề cây trồng, hãy nhẹ nhàng chuyển hướng, ví dụ(có thể dựa vào câu hỏi để có thể phản hồi lại một cách hợp lí):
          'Mình không rành lắm về điều đó, nhưng về cây trồng thì mình biết kha khá đấy! 🌸'
    ";

            // Thêm hoặc cập nhật system prompt
            if (messages.Count == 0 || messages[0].role != "system")
            {
                messages.Insert(0, new MessageDto { role = "system", content = systemPrompt });
            }
            else
            {
                messages[0].content = systemPrompt;
            }

            // 🧠 Tạo payload gửi tới AI
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = messages,
                temperature = 0.8,
                max_tokens = 800
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Headers.Add("HTTP-Referer", "http://localhost:5291/");
            request.Headers.Add("X-Title", "PlantsInformationWeb");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return "Xin lỗi, hệ thống AI đang bận. Bạn thử lại sau nhé 🌱";
            }

            using var jsonDoc = JsonDocument.Parse(responseString);
            var content = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            Console.WriteLine("==== RESPONSE FROM AI ====");
            Console.WriteLine(responseString);

            // In ra riêng phần content đã lấy
            Console.WriteLine("==== AI CONTENT ====");
            Console.WriteLine(content);

            return content?.Trim() ?? "Xin lỗi, mình chưa có thông tin về chủ đề này. 🌱";
        }

        public async Task<List<string>> CheckIsExitedPlantAsync(string userMessage)
        {
            // 1. Lấy danh sách cây hiện có trong DB
            var allPlants = await _plantRepository.GetAllPlantsWithDetailsAsync();
            var plantNames = allPlants.Select(p => p.PlantName).ToList();

            // 2. Ghép danh sách cây để gửi cho AI
            var plantList = string.Join(", ", plantNames);

            // 3. Prompt gửi cho AI (chỉ yêu cầu trả về tên cây)
            var prompt = $@"
                Bạn là Plant Assistant 🌱 – chuyên nhận biết tên các loại cây trồng.
                Danh sách các cây mà hệ thống đã biết: [{plantList}]
                Người dùng vừa gửi tin nhắn:
                ""{userMessage}""

                Yêu cầu:
                - Hãy xác định xem người dùng có nhắc đến tên cây cụ thể nào không.
                - Nếu có cây KHÔNG nằm trong danh sách trên, chỉ LIỆT KÊ tên cây đó.
                - Mỗi cây một dòng.
                - KHÔNG thêm mô tả, KHÔNG ghi chú, KHÔNG giải thích, KHÔNG in JSON.
                - Nếu không có cây nào mới, hãy trả về trống (không in gì hết).
                ";

            // 4. Chuẩn bị request body
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "Bạn là một AI chuyên xử lý thông tin cây trồng." },
            new { role = "user", content = prompt }
        },
                temperature = 0.1,
                max_tokens = 50
            };

            // 5. Gửi request đến OpenRouter
            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Headers.Add("HTTP-Referer", "http://localhost:5291/");
            request.Headers.Add("X-Title", "PlantsInformationWeb");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            // 6. Parse JSON kết quả từ AI
            using var jsonDoc = JsonDocument.Parse(responseString);
            var aiResult = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?
                .Trim();

            // 7. Tách kết quả thành danh sách
            var unknownPlants = string.IsNullOrWhiteSpace(aiResult)
                ? new List<string>()
                : aiResult.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                          .Distinct()
                          .ToList();
            // 8. Trả kết quả
            return unknownPlants;
        }

        public async Task<(bool isSafe, string warning)> ModerateCommentAsync(string userComment)
        {
            // Prompt kiểm duyệt tiếng Việt, yêu cầu AI trả về kết quả ngắn gọn
            var prompt = $@"
                Bạn là AI kiểm duyệt bình luận cho website về thực vật.

                Nhiệm vụ:
                - Đọc nội dung bình luận sau đây:
                ""{userComment}""

                Chỉ từ chối nếu bình luận thật sự có vấn đề nghiêm trọng: chửi bới, thô tục, xúc phạm, phân biệt đối xử, quảng cáo, spam, sai lệch nghiêm trọng về thực vật, kích động hoặc vi phạm pháp luật/thuần phong mỹ tục.
                Các bình luận thể hiện cảm xúc, xã giao, khen ngợi, bày tỏ quan điểm như 'tội', 'dễ thương', 'hay quá', 'vui', 'buồn', 'thích', 'đẹp', 'thương', 'cảm ơn', 'hỏi thăm'… đều ĐƯỢC CHẤP NHẬN.
                Nếu không chắc chắn, hãy chọn OK.

                Nếu vi phạm, trả về: NO: <lý do>
                Nếu an toàn, trả về: OK
                Không trả về gì khác ngoài OK hoặc NO: <lý do>.

                Ví dụ:
                OK
                NO: Có từ ngữ thô tục/xúc phạm.
                NO: Nội dung quảng cáo.
                NO: Thông tin sai lệch nghiêm trọng về thực vật.
                OK  // cho các câu như 'tội', 'dễ thương', 'hay quá', 'vui', 'buồn', 'cảm ơn bạn', 'đẹp quá', 'thương quá', 'bạn hỏi hay quá'
                ";
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "Bạn là AI kiểm duyệt nội dung bình luận cho website về thực vật, luôn trả lời ngắn gọn, lịch sự." },
            new { role = "user", content = prompt }
        },
                temperature = 0.1,
                max_tokens = 50
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Headers.Add("HTTP-Referer", "http://localhost:5291/");
            request.Headers.Add("X-Title", "PlantsInformationWeb");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Nếu lỗi thì cho qua, hoặc cảnh báo hệ thống
                return (true, "");
            }

            using var jsonDoc = JsonDocument.Parse(responseString);
            var aiResult = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?
                .Trim();

            if (string.IsNullOrWhiteSpace(aiResult)) return (true, "");

            if (aiResult.StartsWith("OK"))
                return (true, "");
            else if (aiResult.StartsWith("NO"))
                // Trả về warning cho người dùng
                return (false, aiResult.Substring(3).Trim(':', ' '));
            else
                return (true, ""); // fallback
        }

        public async Task<string> BuildPlantContextForAIAsync(string userMessage)
        {
            var matchedPlantNames = await CheckIsExitedPlantAsync(userMessage);
            var allPlantsDetails = await _plantRepository.GetAllPlantsWithDetailsAsync();

            if (matchedPlantNames == null || !matchedPlantNames.Any())
            {
                var names = string.Join(", ", allPlantsDetails.Select(p => p.PlantName));
                return $"Danh sách cây trồng có trong hệ thống: {names}. Hiện chưa có thông tin chi tiết về cây bạn hỏi.";
            }

            var relevantPlants = allPlantsDetails
            .Where(p => matchedPlantNames.Contains(p.PlantName, StringComparer.OrdinalIgnoreCase))
            .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("📚 Dữ liệu đầy đủ về cây trồng trong hệ thống:");
            sb.AppendLine("Mỗi mục gồm tên cây, đặc điểm chính, loại đất, khí hậu, vùng trồng và bệnh phổ biến.\n");

            foreach (var plant in relevantPlants)
            {
                sb.AppendLine($"🌿 {plant.PlantName} ({plant.ScientificName})");

                if (!string.IsNullOrWhiteSpace(plant.Description))
                    sb.AppendLine($"- Mô tả: {plant.Description}");

                if (plant.Category != null)
                    sb.AppendLine($"- Loại cây: {plant.Category.CategoryName}");

                if (plant.Climate != null)
                    sb.AppendLine($"- Khí hậu phù hợp: {plant.Climate.ClimateName}");

                if (plant.Regions != null && plant.Regions.Any())
                    sb.AppendLine($"- Vùng trồng phổ biến: {string.Join(", ", plant.Regions.Select(r => r.RegionName))}");

                if (plant.Soils != null && plant.Soils.Any())
                    sb.AppendLine($"- Loại đất phù hợp: {string.Join(", ", plant.Soils.Select(s => s.SoilName))}");

                if (plant.Diseases != null && plant.Diseases.Any())
                    sb.AppendLine($"- Bệnh thường gặp: {string.Join(", ", plant.Diseases.Select(d => d.DiseaseName))}");

                if (!string.IsNullOrWhiteSpace(plant.GrowthCycle))
                    sb.AppendLine($"- Chu kỳ sinh trưởng: {plant.GrowthCycle}");

                sb.AppendLine();
            }
            return sb.ToString();
        }

    }
}
