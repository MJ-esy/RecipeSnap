import './HeroSection.css'

export default function HeroSection() {
  return (
    <div className="hero-section">
      <div className="hero-badge">Recipe Conversion</div>

      <h1 className="hero-title">
        Snap a recipe,<br />
        <span className="hero-title-accent">skip the maths.</span>
      </h1>

      <p className="hero-subtitle">
        RecipeSnap reads printed recipes and instantly converts all measurements
        between metric and imperial (US) — grams, cups, Fahrenheit, and more.
      </p>

      <div className="hero-actions">
        <a href="#scan" className="hero-btn-primary">Try it out</a>
      </div>

      <div className="hero-features">
        <div className="hero-feature">
          <span className="hero-feature-icon">📸</span>
          <span>Snap</span>
        </div>
        <div className="hero-feature-divider" />
        <div className="hero-feature">
          <span className="hero-feature-icon">🔄</span>
          <span>Convert</span>
        </div>
        <div className="hero-feature-divider" />
        <div className="hero-feature">
          <span className="hero-feature-icon">🍽️</span>
          <span>Cook</span>
        </div>
      </div>
    </div>
  )
}