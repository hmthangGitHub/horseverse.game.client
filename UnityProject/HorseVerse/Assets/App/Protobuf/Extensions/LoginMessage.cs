using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public sealed partial class LoginMessage : ISubMessage<LoginMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType gameMessageType => GameMessageType.LoginMessage;
        public LoginMessage(LoginRequest request)
        {
            this.msgType_ = LoginMessageType.LoginRequest;
            loginRequest_ = request;
        }

        public LoginMessage(EmailCodeRequest request)
        {
            this.msgType_ = LoginMessageType.EmailCodeRequest;
            emailCodeRequest_ = request;
        }
    }
}