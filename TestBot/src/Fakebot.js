import React, { Component } from 'react';
import './Fakebot.scss';

export default class Fakebot extends Component {
  state={
    response: "Beklager, jeg forstod ikke hva du mente, prøv igjen.",
    question: "",
    conversation: [],
    messageEnd: this.refs.messageEnd
  }


  handleClick = (event) =>{
    event.preventDefault()
    let tempQ = this.state.question
    let {conversation} = this.state
    conversation.push({message: this.state.question, timeStamp: this.getTimeStamp(), type: "question"})

    setTimeout(function() {this.getResponse(tempQ)}.bind(this), 2000)
    this.setState({question: ""})
  }

  getResponse(question){
    question = question.toLowerCase();
    let {response} = this.state
    let {conversation} = this.state

    if(question.includes("lønn")){
      response = <p>Her kan du finne lønnen din: <a href="">random link</a></p>
    }else if(question.includes("hei") || question.includes("hallo")){
      response = <p>Hei på deg. Jeg er Fakebot, jeg kan ikke hjelpe deg med noen verdens ting.</p>
    }else if(question.includes("cv")){
      response = <p>For å finne CV'en din må jeg vite hvem du er. Fortell meg hva du heter.</p>
    }else if(question.includes("hehe") || question.includes("haha")){
      response = <p>Tror du at du er morsom? Jeg forstår meg ikke på sånt.</p>
    }

    conversation.push({message: response, timeStamp: this.getTimeStamp(), type: "response"})
    this.setState(this.state)
    this.scrollToBottomMessage()
  }

  getTimeStamp(){
    const date = new Date()
    const seconds = this.minimumTwoDigits(date.getSeconds())
    const minutes = this.minimumTwoDigits(date.getMinutes())
    const hours = this.minimumTwoDigits(date.getHours())
    return (`${hours}:${minutes}:${seconds}`)
  }

  minimumTwoDigits(number){
    if (number < 10){
      return "0" + number
    }
    return number
  }

  scrollToBottomMessage(){
    document.getElementById("messageEnd").scrollIntoView(true)
  }

  render(){
    return(
      <div className="fakebot">
        <h2 className="header">Fakebot</h2>
        <div className="chat">
          <RenderConversation conversation={this.state.conversation}/>
          <form className="inputContainer" onSubmit={event => this.handleClick(event)}>
            <input className="inputQ" onChange={event => this.setState({question: event.target.value})} value={this.state.question}/>
            <button className="submitQ">Send</button>
          </form>
        </div>
        <div className="exampleQuestions">
          <h3>Spørsmål Fakebot kan svare på:</h3>
          <ul>
            <li>Hei på deg Fakebot</li>
            <li>Hallo bot</li>
            <li>Hvor finner jeg lønnen min?</li>
            <li>Hvor er lønnen?</li>
            <li>Kan jeg få se CV'en min?</li>
            <li>CV'en min, hvor finner jeg den?</li>
          </ul>
        </div>

      </div>
    )
  }
}

function RenderConversation(props){
  const conversation = props.conversation.map((message) =>
    <div className="messageLine"><p className={`${message.type} message`}>{message.message}<br/><small className={`${message.type} timestamp`}>{message.timeStamp}</small></p></div>
  )
  return(
    <div className="chatWindow">{conversation} <div id="messageEnd"/></div>
  )
}
