import { useState } from "react";
import { useNavigate } from "react-router-dom";
import * as userApi from "../api/userApi";
import * as barApi from "../api/barApi";
import type {
  BarsWithinDistanceResponse,
  DbBar,
  NearbyBar,
} from "../types/barRec";

const USER_ID = 99999;
const USER_LAT = 54.8985;
const USER_LON = 23.9036;
const INITIAL_DISTANCE_METERS = 10000;

type Stage =
  | { kind: "idle" }
  | { kind: "loading" }
  | { kind: "error"; message: string }
  | { kind: "results"; bars: NearbyBar[]; dbBars: DbBar[] };

export default function BarRecPage() {
  const navigate = useNavigate();
  const [stage, setStage] = useState<Stage>({ kind: "idle" });
  const [currentDistance, setCurrentDistance] = useState(
    INITIAL_DISTANCE_METERS,
  );

  async function pressStart() {
    const { surveyNeeded } = await userApi.openSurveyForm(USER_ID);

    if (surveyNeeded) {
      navigate("/taste-survey");
      return;
    }

    setCurrentDistance(INITIAL_DISTANCE_METERS);
    requestBarsWithinDistance(INITIAL_DISTANCE_METERS);
  }

  async function requestBarsWithinDistance(distanceMeters: number) {
    setStage({ kind: "loading" });

    let response: BarsWithinDistanceResponse | null = null;

    try {
      response = await barApi.requestBarsWithinDistance(
        USER_ID,
        USER_LAT,
        USER_LON,
        distanceMeters,
      );
    } catch (e) {
      // (13-14) ir (24-25)
      displayError(e instanceof Error ? e.message : "Failed to fetch bars.");
    }

    // response within-distance
    const allBarsEvaluated = checkAllBarsEvaluated(response);

    if (allBarsEvaluated === false && response !== null) {
      // (44-45)
      displayError("Not all bars were evaluated");
    }

    if (allBarsEvaluated === true && response !== null) {
      // (46-48)
      const sortedBars = sortBarsByCalculatedRating(response);
      setStage({ kind: "results", bars: response.bars, dbBars: sortedBars });
    }
  }

  function displayError(message: string) {
    setStage({ kind: "error", message });
  }

  function checkAllBarsEvaluated(
    response: BarsWithinDistanceResponse | null,
  ): boolean {
    return response !== null && response.dbBars.length > 0;
  }

  function sortBarsByCalculatedRating(
    response: BarsWithinDistanceResponse,
  ): DbBar[] {
    return response.dbBars;
  }

  function tryAgain() {
    const newDistance = currentDistance + 1000;
    setCurrentDistance(newDistance);
    requestBarsWithinDistance(newDistance);
  }

  function tryAgainError() {
    requestBarsWithinDistance(currentDistance);
  }

  if (stage.kind === "loading") {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Bar <span>Recommendations</span>
          </h2>
          <p>Searching nearby bars...</p>
        </div>
      </div>
    );
  }

  if (stage.kind === "error") {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            Something went <span>Wrong</span>
          </h2>
          <div className="bar-form__error">{stage.message}</div>
          <div className="bar-form__actions">
            <button
              className="btn btn--ghost"
              onClick={() => navigate("/")}
            >
              Home
            </button>
            <button
              className="btn btn--primary"
              onClick={tryAgainError}
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (stage.kind === "results") {
    return (
      <div className="bar-form-page">
        <div className="bar-form bar-form--wide">
          <h2 className="bar-form__title">
            Recommended <span>Bars</span>
          </h2>
          <p className="bar-form__subtitle">
            Search radius: {currentDistance / 1000} km · {stage.dbBars.length}{" "}
            bars found
          </p>
          <ul className="bar-rec-list">
            {stage.dbBars.map((bar, i) => (
              <li
                key={bar.id}
                className="bar-rec-list__item"
              >
                <div className="bar-rec-list__header">
                  <span className="bar-rec-list__name">{bar.name}</span>
                  {i === 0 && bar.priority > 0 && (
                    <span className="bar-rec-list__top">TOP MATCH</span>
                  )}
                </div>
                <div className="bar-rec-list__meta">
                  <span className="bar-rec-list__rating">
                    ★ {bar.rating.toFixed(1)}
                  </span>
                  <span className="bar-rec-list__design">{bar.design}</span>
                  <span className="bar-rec-list__priority">
                    {bar.priority} / 10 match
                  </span>
                </div>
              </li>
            ))}
          </ul>
          <div className="bar-form__actions">
            <button
              className="btn btn--ghost"
              onClick={() => navigate("/")}
            >
              Home
            </button>
            <button
              className="btn btn--primary"
              onClick={tryAgain}
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">
          Bar <span>Recommendations</span>
        </h2>
        <p>Find a bar that matches your taste.</p>
        <div className="bar-form__actions">
          <button
            className="btn btn--primary"
            onClick={pressStart}
          >
            Start
          </button>
        </div>
      </div>
    </div>
  );
}
