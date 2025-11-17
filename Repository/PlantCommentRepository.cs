using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class PlantCommentRepository : IPlantCommentRepository
    {
        private readonly PlantsInformationContext _context;
        private readonly IMapper _mapper;


        public PlantCommentRepository(PlantsInformationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PlantCommentDto>> GetCommentsByPlantId(int plantId)
        {
            var comments = await _context.Plantcomments
                .Where(c => c.PlantId == plantId && c.IsDeleted == false)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var commentDict = comments.ToDictionary(c => c.CommentId);

            // Tạo dictionary lưu replies cho từng comment
            var repliesDict = new Dictionary<int, List<Plantcomment>>();

            foreach (var comment in comments)
            {
                if (comment.ParentCommentId.HasValue)
                {
                    if (!repliesDict.ContainsKey(comment.ParentCommentId.Value))
                        repliesDict[comment.ParentCommentId.Value] = new List<Plantcomment>();
                    repliesDict[comment.ParentCommentId.Value].Add(comment);
                }
            }

            // Hàm đệ quy để map comment và replies sang DTO
            PlantCommentDto MapComment(Plantcomment comment)
            {
                var dto = _mapper.Map<PlantCommentDto>(comment);
                if (repliesDict.TryGetValue(comment.CommentId, out var replies))
                    dto.Replies = replies.Select(MapComment).ToList();
                else
                    dto.Replies = new List<PlantCommentDto>();
                return dto;
            }

            var rootComments = comments.Where(c => c.ParentCommentId == null);

            // Trả về danh sách comment gốc đã lồng replies
            return rootComments.Select(MapComment).ToList();
        }

        public async Task<Plantcomment> GetCommentWithUserByIdAsync(int commentId)
        {
            return await _context.Plantcomments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }
    }
}