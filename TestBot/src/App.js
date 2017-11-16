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
          <p>Burde vel ha noe tekst her som forklarer formålet bak prosjektet og hvorfor vi trenger help til å lage spørsmål.</p>
        </header>
        <div className="mainContainer">
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
