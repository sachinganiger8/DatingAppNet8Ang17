using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
        .Where(w => w.SourceUserId == currentUserId)
        .Select(s => s.TargetUderId)
        .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targteUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targteUserId);
    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                .Where(w => w.SourceUserId == likesParams.UserId)
                .Select(s => s.TargetUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likes
                .Where(w => w.TargetUderId == likesParams.UserId)
                .Select(s => s.SourceUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                .Where(w => w.TargetUderId == likesParams.UserId && likeIds.Contains(w.SourceUserId))
                .Select(s => s.SourceUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
