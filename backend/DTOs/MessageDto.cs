namespace backend.DTOs;

public record MessageDto(int Id, int SenderId, string SenderUsername, string Content, DateTime SentAt);
public record SendMessageDto(int SenderId, int ReceiverId, string Content);
