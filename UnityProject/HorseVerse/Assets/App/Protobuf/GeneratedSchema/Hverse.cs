// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: hverse.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace io.hverse.game.protogen {

  /// <summary>Holder for reflection information generated from hverse.proto</summary>
  public static partial class HverseReflection {

    #region Descriptor
    /// <summary>File descriptor for hverse.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static HverseReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxodmVyc2UucHJvdG8aGWdvb2dsZS9wcm90b2J1Zi9hbnkucHJvdG8iVwoL",
            "R2FtZU1lc3NhZ2USIQoHbXNnVHlwZRgBIAEoDjIQLkdhbWVNZXNzYWdlVHlw",
            "ZRIlCgdtc2dEYXRhGAIgASgLMhQuZ29vZ2xlLnByb3RvYnVmLkFueSqeAQoP",
            "R2FtZU1lc3NhZ2VUeXBlEhIKDlNZU1RFTV9NRVNTQUdFEAASEAoMUElOR19N",
            "RVNTQUdFEAESFwoTTUFTVEVSX0RBVEFfTUVTU0FHRRACEhEKDUxPR0lOX01F",
            "U1NBR0UQAxISCg5QTEFZRVJfTUVTU0FHRRAEEhAKDFJBQ0VfTUVTU0FHRRAF",
            "EhMKD0JFVFRJTkdfTUVTU0FHRRAGQkkKF2lvLmh2ZXJzZS5nYW1lLnByb3Rv",
            "Z2VuQhRIdmVyc2VNZXNzYWdlRmFjdG9yeaoCF2lvLmh2ZXJzZS5nYW1lLnBy",
            "b3RvZ2VuYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.AnyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::io.hverse.game.protogen.GameMessageType), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::io.hverse.game.protogen.GameMessage), global::io.hverse.game.protogen.GameMessage.Parser, new[]{ "MsgType", "MsgData" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum GameMessageType {
    [pbr::OriginalName("SYSTEM_MESSAGE")] SystemMessage = 0,
    [pbr::OriginalName("PING_MESSAGE")] PingMessage = 1,
    [pbr::OriginalName("MASTER_DATA_MESSAGE")] MasterDataMessage = 2,
    [pbr::OriginalName("LOGIN_MESSAGE")] LoginMessage = 3,
    [pbr::OriginalName("PLAYER_MESSAGE")] PlayerMessage = 4,
    [pbr::OriginalName("RACE_MESSAGE")] RaceMessage = 5,
    [pbr::OriginalName("BETTING_MESSAGE")] BettingMessage = 6,
  }

  #endregion

  #region Messages
  public sealed partial class GameMessage : pb::IMessage<GameMessage>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<GameMessage> _parser = new pb::MessageParser<GameMessage>(() => new GameMessage());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<GameMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::io.hverse.game.protogen.HverseReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameMessage() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameMessage(GameMessage other) : this() {
      msgType_ = other.msgType_;
      msgData_ = other.msgData_ != null ? other.msgData_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameMessage Clone() {
      return new GameMessage(this);
    }

    /// <summary>Field number for the "msgType" field.</summary>
    public const int MsgTypeFieldNumber = 1;
    private global::io.hverse.game.protogen.GameMessageType msgType_ = global::io.hverse.game.protogen.GameMessageType.SystemMessage;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::io.hverse.game.protogen.GameMessageType MsgType {
      get { return msgType_; }
      set {
        msgType_ = value;
      }
    }

    /// <summary>Field number for the "msgData" field.</summary>
    public const int MsgDataFieldNumber = 2;
    private global::Google.Protobuf.WellKnownTypes.Any msgData_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.WellKnownTypes.Any MsgData {
      get { return msgData_; }
      set {
        msgData_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as GameMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(GameMessage other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MsgType != other.MsgType) return false;
      if (!object.Equals(MsgData, other.MsgData)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (MsgType != global::io.hverse.game.protogen.GameMessageType.SystemMessage) hash ^= MsgType.GetHashCode();
      if (msgData_ != null) hash ^= MsgData.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (MsgType != global::io.hverse.game.protogen.GameMessageType.SystemMessage) {
        output.WriteRawTag(8);
        output.WriteEnum((int) MsgType);
      }
      if (msgData_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(MsgData);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (MsgType != global::io.hverse.game.protogen.GameMessageType.SystemMessage) {
        output.WriteRawTag(8);
        output.WriteEnum((int) MsgType);
      }
      if (msgData_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(MsgData);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (MsgType != global::io.hverse.game.protogen.GameMessageType.SystemMessage) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MsgType);
      }
      if (msgData_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(MsgData);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(GameMessage other) {
      if (other == null) {
        return;
      }
      if (other.MsgType != global::io.hverse.game.protogen.GameMessageType.SystemMessage) {
        MsgType = other.MsgType;
      }
      if (other.msgData_ != null) {
        if (msgData_ == null) {
          MsgData = new global::Google.Protobuf.WellKnownTypes.Any();
        }
        MsgData.MergeFrom(other.MsgData);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            MsgType = (global::io.hverse.game.protogen.GameMessageType) input.ReadEnum();
            break;
          }
          case 18: {
            if (msgData_ == null) {
              MsgData = new global::Google.Protobuf.WellKnownTypes.Any();
            }
            input.ReadMessage(MsgData);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            MsgType = (global::io.hverse.game.protogen.GameMessageType) input.ReadEnum();
            break;
          }
          case 18: {
            if (msgData_ == null) {
              MsgData = new global::Google.Protobuf.WellKnownTypes.Any();
            }
            input.ReadMessage(MsgData);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
