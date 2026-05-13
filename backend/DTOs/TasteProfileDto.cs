namespace backend.DTOs;

public record TasteQuestionDto(string Key, string Text, List<string> Options, int TotalCount);

public record TasteAnswerDto(string QuestionKey, string Answer);

public record SubmitTasteProfileRequest(List<TasteAnswerDto> Answers);

public record TasteProfileResponse(
    int Id,
    int UserId,
    DateTime CreatedAt,
    List<TasteAnswerDto> Answers);
