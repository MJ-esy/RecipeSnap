import './App.css'
import HeroSection from './components/layouts/HeroSection'
import ScanApp from './components/ScanApp'
import Footer from './components/layouts/Footer'
import Instructions from './components/Instructions'

function App() {
  return (
    <>

      <section id="center">
        <HeroSection />
      </section>

      <section id="next-steps">
        <div id="instructions">
          <h2>Instructions</h2>
          <Instructions />

        </div>
        <div id="scan">
          <h2 className='mb-3'>Try RecipeSnap</h2>
          <ScanApp />
        </div>
      </section>

      <Footer />
    </>
  )
}

export default App
