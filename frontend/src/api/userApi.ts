import type {
  TasteQuestion,
  SubmitTasteProfileRequest,
  TasteProfileResponse,
} from '../types/taste';

const BASE = 'http://localhost:5029/api/User';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const nextQuestion = (index: number): Promise<TasteQuestion> =>
  fetch(`${BASE}/next-question/${index}`).then(handleResponse<TasteQuestion>);

export const openSurveyForm = (userId: number): Promise<{ surveyNeeded: boolean }> =>
  fetch(`${BASE}/${userId}/survey-form`).then(handleResponse<{ surveyNeeded: boolean }>);

export const submit = (
  userId: number,
  request: SubmitTasteProfileRequest,
): Promise<TasteProfileResponse> =>
  fetch(`${BASE}/${userId}/taste-profile`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  }).then(handleResponse<TasteProfileResponse>);

export const fetchTasteProfile = (userId: number): Promise<TasteProfileResponse> =>
  fetch(`${BASE}/${userId}/taste-profile`).then(handleResponse<TasteProfileResponse>);

export const deleteTasteProfile = async (userId: number): Promise<void> => {
  const res = await fetch(`${BASE}/${userId}/taste-profile`, { method: 'DELETE' });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
};
