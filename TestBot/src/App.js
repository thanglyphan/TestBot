import React, { Component } from 'react';
import axios from 'axios'
import './App.css';

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
            <p> Vi er trainee gruppe A som igjennom hospitering i tech avdelingen har fått i oppgave å lage en basisløsning for chatbot.
                Noe som vi potensielt kan selge til kunder på senere tidspunkt.
              <br/><br/>
                Ettersom folk generelt skriver veldig ulikt og formulerer seg anderledes er det viktig å trene en chatbot så den forstår hva en bruker ønsker, samme hvordan en person ordlegger seg.
                Det å trene en chatbot er derfor en omfattende jobb, og vi trenger derfor deres hjelp til å formulere spørsmål.
            </p>
          </div>
        </header>
        <div className="mainContainer">
          <div className="desc">
            <p> Til å begynne med har vi bestemt oss for å trene bot'en for HR relaterte spørsmål.
                Vi håper at noen av dere har lyst til å skrive noen spørsmål i boksen nedenfor relatert til dette.
                Disse spørsmålene blir sendt til vår Wit.ai konto, hvor bot'en foreslår meningen bak spørsmålet og hvilken kategori spørsmålet tilhører.
                <br/><br/>
                Eksempel på hvordan bot'en fungerer: <br/> "Hvor <i className="intent">mange</i> <i className="category">feriedager</i> har jeg igjen?"
                <br/><br/>
                Bot'en identifiserer: <br/> mening: <i className="intent">antall</i><br/> kategori: <i className="category">feriedager</i>
                <br/><br/>
                Forslag til flere spørsmål: Adresse, ansatte, cv, sykedager, printer, åpningstider etc.
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
