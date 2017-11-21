import React, { Component } from 'react';
import axios from 'axios'
import './App.min.css';

import Fakebot from "./Fakebot"

class App extends Component {
  state={
    question: "",
    latestQuestions: [],
    failSubmitClass: "",
  }

  onSubmit = (e) => {
    e.preventDefault();
    if(!this.state.question){
      this.setState({failSubmitClass: "fail"})
      return
    }

    let {latestQuestions} = this.state
    latestQuestions.unshift(this.state.question)

    axios({
      url: `https://api.wit.ai/message?v=20171116&q=${this.state.question}`,
      method: 'post',
      headers: {'Authorization': 'Bearer OIU5PFTZFKMMTV5YJL2ZWFCIHBIZ5VF6'},
    }).then(res => {console.log(res)})


    this.setState({question: ""})
  }

  render() {
    return (
      <div className="App">
        <header className="App-header">
          <h1 className="App-title">Chatbot Trainer!</h1>
          <div className="desc">
            <p> Gjennom hospitering i teknologiavdelingen fikk traineegruppe A i oppgave å lage en basisløsning for Creunas helt egne chatbot.
              Løsningen tar utgangspunkt i å kunne bli solgt til kunder i nær fremtid.
              <br/><br/>
                Menneskers måter å uttrykke seg på varierer i stor grad.
                Vi snakker ulikt, vi skriver ulikt, vi forstår ulikt.
                Ja – vi kommuniserer ulikt.
                Det kanskje viktigste arbeidet rundt utviklingen av en chatbot er derfor å trene den opp.
                Mer konkret trener vi den slik at den forstår hva en bruker søker svar på ved å bruke chatboten, samme hvordan denne brukeren ordlegger seg.
                Til dette arbeidet søker vi deres hjelp.
            </p>
          </div>
        </header>
        <div className="mainContainer">
          <div className="desc">
            <p> Med et mål om at chatboten svarer virkelighetens brukere så godt som mulig på deres behov, trenger vi at dere skriver til oss hvordan dere ville stilt spørsmål under et valgt tema.
              Til å begynne med har vi bestemt oss for å trene bot'en på interne spørsmål i Creuna.
              Spørsmålene du skriver i boksen under blir sendt til vår Wit.ai konto, hvor bot'en foreslår meningen bak spørsmålet og hvilken kategori spørsmålet tilhører.
              <br/><br/>
              Merk: ikke tenk på formuleringen din. Vi vil ha ekte spørsmål.
              Gjerne de du skriver nettopp uten å tenke – annet enn at du tenker du trenger svar på noe. Skriv naturlig.
                <br/><br/>
                Eksempel på hvordan bot'en fungerer: "<i className="intent">Hvor finner</i> jeg <i className="category">CV'en</i> min?"
                <br/><br/>
                Bot identifiserer: <br/>
                &nbsp;&nbsp;&nbsp;&ndash; mening: <i className="intent">plassering</i><br/>
                &nbsp;&nbsp;&nbsp;&ndash; kategori: <i className="category">gjenstand</i>
                <br/><br/>
                Forslag til flere spørsmål: Fagområder, ansatte, cv, nøkkelkort, printer, åpningstider etc.
            </p>
          </div>
          <form className="form" onSubmit={e => this.onSubmit(e)}>
            <input
              className={`textInput ${this.state.failSubmitClass}`}
              placeholder="Skriv ditt spørsmål her..."
              value={this.state.question}
              onChange={event => this.setState({question: event.target.value})}
            />
            <button className="submitBtn">Legg til!</button>
          </form>
          <div className="recentQuestions">
            {this.state.latestQuestions[0]
              ? <h2>Dine siste spørsmål:</h2>
              : null
            }
            <GetLatestQuestions questions={this.state.latestQuestions}/>
          </div>
        </div>
        <Fakebot/>
      </div>
    );
  }
}

function GetLatestQuestions(props){
  const questions = props.questions.slice(0, 10).map((question) =>
    <li>{question}</li>
  )
  return(
    <ul>{questions}</ul>
  )
}

export default App;
