import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as api from "../api/userApi";
import type { TasteQuestion } from "../types/taste";

const USER_ID = 1;

type Stage =
  | { kind: "loading" }
  | { kind: "survey"; currentIndex: number }
  | { kind: "review" }
  | { kind: "submitting" }
  | { kind: "success" }
  | { kind: "error"; message: string };

export default function TasteSurvey() {
  const navigate = useNavigate();

  const [questions, setQuestions] = useState<Record<number, TasteQuestion>>({});
  const [totalCount, setTotalCount] = useState(0);
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [stage, setStage] = useState<Stage>({ kind: "loading" });

  async function nextQuestion(index: number): Promise<TasteQuestion> {
    if (questions[index]) return questions[index];
    const q = await api.nextQuestion(index);
    setQuestions((prev) => ({ ...prev, [index]: q }));
    setTotalCount(q.totalCount);
    return q;
  }

  useEffect(() => {
    nextQuestion(0)
      .then(() => setStage({ kind: "survey", currentIndex: 0 }))
      .catch((e) =>
        setStage({
          kind: "error",
          message: e instanceof Error ? e.message : "Failed to load questions.",
        }),
      );
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function submitAnswer(option: string) {
    if (stage.kind !== "survey") return;
    const question = questions[stage.currentIndex];
    setAnswers({ ...answers, [question.key]: option });

    const nextIndex = stage.currentIndex + 1;
    if (nextIndex >= totalCount) {
      setStage({ kind: "review" });
      return;
    }

    try {
      await nextQuestion(nextIndex);
      setStage({ kind: "survey", currentIndex: nextIndex });
    } catch (e) {
      setStage({
        kind: "error",
        message:
          e instanceof Error ? e.message : "Failed to load next question.",
      });
    }
  }

  function goBack() {
    if (stage.kind === "survey" && stage.currentIndex > 0) {
      setStage({ kind: "survey", currentIndex: stage.currentIndex - 1 });
    } else if (stage.kind === "review") {
      setStage({ kind: "survey", currentIndex: totalCount - 1 });
    }
  }

  async function submitSurvey() {
    setStage({ kind: "submitting" });
    try {
      const orderedAnswers = Array.from(
        { length: totalCount },
        (_, i) => questions[i],
      ).map((q) => ({ questionKey: q.key, answer: answers[q.key] }));
      await api.submit(USER_ID, { answers: orderedAnswers });
      setStage({ kind: "success" });
    } catch (e) {
      setStage({
        kind: "error",
        message: e instanceof Error ? e.message : "Failed to save.",
      });
    }
  }

  function displaySuccess() {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Profile <span>Saved</span>
          </h2>
          <p>Your taste profile has been saved!</p>
          <div className="bar-form__actions">
            <button
              className="btn btn--primary"
              onClick={returnToPreviousPage}
            >
              Back
            </button>
          </div>
        </div>
      </div>
    );
  }

  function returnToPreviousPage() {
    navigate(-1);
  }

  function displayError(message: string) {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Something went <span>Wrong</span>
          </h2>
          <div className="bar-form__error">{message}</div>
          <div className="bar-form__actions">
            <button
              className="btn btn--primary"
              onClick={returnToPreviousPage}
            >
              Back
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (stage.kind === "loading") {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Taste <span>Survey</span>
          </h2>
          <p>Loading...</p>
        </div>
      </div>
    );
  }

  if (stage.kind === "submitting") {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Taste <span>Survey</span>
          </h2>
          <p>Saving...</p>
        </div>
      </div>
    );
  }

  if (stage.kind === "error") return displayError(stage.message);
  if (stage.kind === "success") return displaySuccess();

  if (stage.kind === "review") {
    const orderedQuestions = Array.from(
      { length: totalCount },
      (_, i) => questions[i],
    );
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Review your <span>Answers</span>
          </h2>
          <ul className="taste-survey__review">
            {orderedQuestions.map((q) => (
              <li key={q.key}>
                <strong>{q.text}</strong>
                <div className="taste-survey__review-answer">
                  {answers[q.key]}
                </div>
              </li>
            ))}
          </ul>
          <div className="bar-form__actions">
            <button
              className="btn btn--ghost"
              onClick={goBack}
            >
              Back
            </button>
            <button
              className="btn btn--primary"
              onClick={submitSurvey}
            >
              Submit
            </button>
          </div>
        </div>
      </div>
    );
  }

  const question = questions[stage.currentIndex];
  const currentAnswer = answers[question.key];
  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">
          Taste <span>Survey</span>
        </h2>
        <div className="bar-form__field">
          <label className="bar-form__label">
            Question {stage.currentIndex + 1} of {totalCount}
          </label>
          <div className="taste-survey__question-text">{question.text}</div>
          <div className="taste-survey__options">
            {question.options.map((opt) => (
              <button
                key={opt}
                className={`taste-survey__option ${currentAnswer === opt ? "taste-survey__option--selected" : ""}`}
                onClick={() => submitAnswer(opt)}
              >
                {opt}
              </button>
            ))}
          </div>
        </div>
        {stage.currentIndex > 0 && (
          <div className="bar-form__actions">
            <button
              className="btn btn--ghost"
              onClick={goBack}
            >
              Back
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
