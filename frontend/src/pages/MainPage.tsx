import { useNavigate } from "react-router-dom";

export default function MainPage() {
  const navigate = useNavigate();

  function openBarRecommendationPage() {
    navigate("/bar-recommendation");
  }

  function openFriendsPage() {
    navigate("/friends");
  }

  function openBarRoutePage() {
    navigate("/bar-route");
  }

  return (
    <div className="main-page">
      <div
        style={{
          textAlign: "center",
          display: "flex",
          flexDirection: "column",
          gap: "12px",
        }}
      >
        <h1 className="main-page__title">
          Bar<span>System</span>
        </h1>
      </div>
      <div className="main-page__nav">
        <button
          className="btn--primary-lg"
          onClick={() => navigate("/bars")}
        >
          Open Bars
        </button>
        <button
          className="btn--primary-lg"
          onClick={() => navigate("/reservations")}
        >
          Make Reservation
        </button>
        <button
          className="btn--primary-lg"
          onClick={openBarRecommendationPage}
        >
          Recommend Bars
        </button>
        <button
          className="btn--primary-lg"
          onClick={() => navigate("/recipes")}
        >
          Cocktail Recipes
        </button>
        <button
          className="btn--primary-lg btn--primary-lg--green"
          onClick={() => navigate("/blackjack")}
        >
          Blackjack
        </button>
        <button
          className="btn--primary-lg"
          onClick={openFriendsPage}
        >
          Friends
        </button>
        <button
          className="btn--primary-lg"
          onClick={openBarRoutePage}
        >
          Bar Route
        </button>
      </div>
    </div>
  );
}
