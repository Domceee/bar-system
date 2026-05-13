export interface TasteQuestion {
  key: string;
  text: string;
  options: string[];
  totalCount: number;
}

export interface TasteAnswer {
  questionKey: string;
  answer: string;
}

export interface SubmitTasteProfileRequest {
  answers: TasteAnswer[];
}

export interface TasteProfileResponse {
  id: number;
  userId: number;
  createdAt: string;
  answers: TasteAnswer[];
}
