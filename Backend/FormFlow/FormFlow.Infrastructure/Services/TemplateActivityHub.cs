using FormFlow.Infrastructure.Models.SignalREventModels;
using FormFlow.Application.DTOs.Comments;
using FormFlow.Application.Interfaces;
using FormFlow.Infrastructure.Models;
using Microsoft.AspNetCore.SignalR;

namespace FormFlow.Infrastructure.Services
{
    public class TemplateActivityHub : Hub
    {
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly ITemplateService _templateService;

        public TemplateActivityHub(
            ICommentService commentService,
            ILikeService likeService,
            ITemplateService templateService)
        {
            _commentService = commentService;
            _likeService = likeService;
            _templateService = templateService;
        }

        public async Task JoinTemplateGroup(Guid templateId)
        {
            try
            {
                var currentUser = GetCurrentUser();

                if (!await _templateService.HasUserAccessToTemplateAsync(templateId, currentUser?.Id ?? Guid.Empty))
                {
                    await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                    {
                        Message = "No access to this template"
                    });
                    return;
                }

                var groupName = GetTemplateGroupName(templateId);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                if (currentUser != null)
                {
                    await Clients.Group(groupName).SendAsync(Constants.SignalREvents.USER_JOINED, new UserJoinedEvent
                    {
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        TemplateId = templateId,
                        JoinedAt = DateTime.UtcNow
                    });
                }

                await Clients.Caller.SendAsync(Constants.SignalREvents.JOINED_TEMPLATE, new JoinedTemplateEvent
                {
                    TemplateId = templateId,
                    Message = "Successfully joined template activity"
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = "Failed to join template group",
                    ErrorCode = "JOIN_TEMPLATE_ERROR"
                });
            }
        }

        public async Task LeaveTemplateGroup(Guid templateId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                var groupName = GetTemplateGroupName(templateId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                if (currentUser != null)
                {
                    await Clients.Group(groupName).SendAsync(Constants.SignalREvents.USER_LEFT, new UserLeftEvent
                    {
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        TemplateId = templateId,
                        LeftAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = "Failed to leave template group",
                    ErrorCode = "LEAVE_TEMPLATE_ERROR"
                });
            }
        }

        public async Task AddComment(Guid templateId, string content)
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser == null)
                {
                    await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                    {
                        Message = "Authentication required to add comments",
                        ErrorCode = "AUTH_REQUIRED"
                    });
                    return;
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                    {
                        Message = "Comment content cannot be empty",
                        ErrorCode = "EMPTY_CONTENT"
                    });
                    return;
                }

                var request = new AddCommentRequest
                {
                    TemplateId = templateId,
                    Content = content.Trim()
                };

                var comment = await _commentService.AddCommentAsync(currentUser.Id, request);
                var groupName = GetTemplateGroupName(templateId);

                await Clients.Group(groupName).SendAsync(Constants.SignalREvents.NEW_COMMENT, new NewCommentEvent
                {
                    Comment = comment,
                    TemplateId = templateId,
                    AddedAt = DateTime.UtcNow
                });

                await Clients.Caller.SendAsync(Constants.SignalREvents.COMMENT_ADDED, new CommentAddedEvent
                {
                    Success = true,
                    Comment = comment
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = ex.Message,
                    ErrorCode = "ADD_COMMENT_ERROR"
                });
            }
        }

        public async Task ToggleLike(Guid templateId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser == null)
                {
                    await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                    {
                        Message = "Authentication required to toggle likes",
                        ErrorCode = "AUTH_REQUIRED"
                    });
                    return;
                }

                var result = await _likeService.ToggleLikeAsync(currentUser.Id, templateId);
                var groupName = GetTemplateGroupName(templateId);

                await Clients.Group(groupName).SendAsync(Constants.SignalREvents.LIKE_TOGGLED, new LikeToggledEvent
                {
                    TemplateId = templateId,
                    TotalLikes = result.TotalLikes,
                    IsLiked = result.IsLiked,
                    Action = result.Action,
                    UserId = currentUser.Id,
                    UserName = currentUser.UserName,
                    UpdatedAt = DateTime.UtcNow
                });

                await Clients.Caller.SendAsync(Constants.SignalREvents.LIKE_RESULT, new LikeResultEvent
                {
                    Success = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = ex.Message,
                    ErrorCode = "TOGGLE_LIKE_ERROR"
                });
            }
        }

        public async Task GetTemplateActivity(Guid templateId)
        {
            try
            {
                var currentUser = GetCurrentUser();

                if (!await _templateService.HasUserAccessToTemplateAsync(templateId, currentUser?.Id ?? Guid.Empty))
                {
                    await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                    {
                        Message = "No access to this template",
                        ErrorCode = "ACCESS_DENIED"
                    });
                    return;
                }

                var recentComments = await _commentService.GetRecentCommentsAsync(templateId, 10);
                var likesCount = await _likeService.GetLikesCountAsync(templateId);
                var userLiked = currentUser != null ? await _likeService.HasUserLikedAsync(currentUser.Id, templateId) : false;

                await Clients.Caller.SendAsync(Constants.SignalREvents.TEMPLATE_ACTIVITY, new TemplateActivityEvent
                {
                    TemplateId = templateId,
                    RecentComments = recentComments,
                    LikesCount = likesCount,
                    UserLiked = userLiked,
                    LoadedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = "Failed to load template activity",
                    ErrorCode = "LOAD_ACTIVITY_ERROR"
                });
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var currentUser = GetCurrentUser();
                await Clients.Caller.SendAsync(Constants.SignalREvents.CONNECTED, new ConnectedEvent
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = currentUser?.Id,
                    UserName = currentUser?.UserName,
                    IsAuthenticated = currentUser != null,
                    ConnectedAt = DateTime.UtcNow
                });

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync(Constants.SignalREvents.ERROR, new ErrorEvent
                {
                    Message = "Connection failed",
                    ErrorCode = "CONNECTION_ERROR"
                });
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var currentUser = GetCurrentUser();
                if (currentUser != null)
                {
                    await Clients.All.SendAsync(Constants.SignalREvents.USER_DISCONNECTED, new UserDisconnectedEvent
                    {
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName,
                        DisconnectedAt = DateTime.UtcNow
                    });
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch
            {
                await base.OnDisconnectedAsync(exception);
            }
        }

        private CurrentUser? GetCurrentUser()
        {
            return Context.GetHttpContext()?.Items[Constants.Auth.CURRENT_USER_KEY] as CurrentUser;
        }

        private static string GetTemplateGroupName(Guid templateId)
        {
            return $"template_{templateId}";
        }
    }
}
