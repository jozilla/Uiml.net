
using System;
using System.IO;
using System.Collections;


namespace Cassowary.Parsing
{

  public class Token {
    public int kind;    // token kind
    public int pos;     // token position in the source text (starting at 0)
    public int col;     // token column (starting at 0)
    public int line;    // token line (starting at 1)
    public string val;  // token value
    public Token next;  // ML 2005-03-11 Tokens are kept in linked list
  }

  public class Buffer {
    public const char EOF = (char)256;
    const int MAX_BUFFER_LENGTH = 64 * 1024; // 64KB
    byte[] buf;         // input buffer
    int bufStart;       // position of first byte in buffer relative to input stream
    int bufLen;         // length of buffer
    int fileLen;        // length of input stream
    int pos;            // current position in buffer
    Stream stream;      // input stream (seekable)
    bool isUserStream;  // was the stream opened by the user?
    
    public Buffer (Stream s, bool isUserStream) {
      stream = s; this.isUserStream = isUserStream;
      fileLen = bufLen = (int) s.Length;
      if (stream.CanSeek && bufLen > MAX_BUFFER_LENGTH) bufLen = MAX_BUFFER_LENGTH;
      buf = new byte[bufLen];
      bufStart = Int32.MaxValue; // nothing in the buffer so far
      Pos = 0; // setup buffer to position 0 (start)
      if (bufLen == fileLen) Close();
    }
    
    ~Buffer() { Close(); }
    
    void Close() {
      if (!isUserStream && stream != null) {
        stream.Close();
        stream = null;
      }
    }
    
    public int Read () {
      if (pos < bufLen) {
        return buf[pos++];
      } else if (Pos < fileLen) {
        Pos = Pos; // shift buffer start to Pos
        return buf[pos++];
      } else {
        return EOF;
      }
    }

    public int Peek () {
      if (pos < bufLen) {
        return buf[pos];
      } else if (Pos < fileLen) {
        Pos = Pos; // shift buffer start to Pos
        return buf[pos];
      } else {
        return EOF;
      }
    }
    
    public string GetString (int beg, int end) {
      int len = end - beg;
      char[] buf = new char[len];
      int oldPos = Pos;
      Pos = beg;
      for (int i = 0; i < len; i++) buf[i] = (char) Read();
      Pos = oldPos;
      return new String(buf);
    }

    public int Pos {
      get { return pos + bufStart; }
      set {
        if (value < 0) value = 0; 
        else if (value > fileLen) value = fileLen;
        if (value >= bufStart && value < bufStart + bufLen) { // already in buffer
          pos = value - bufStart;
        } else if (stream != null) { // must be swapped in
          stream.Seek(value, SeekOrigin.Begin);
          bufLen = stream.Read(buf, 0, buf.Length);
          bufStart = value; pos = 0;
        } else {
          pos = fileLen - bufStart; // make Pos return fileLen
        }
      }
    }
  }

  public class Scanner {
    const char EOL = '\n';
    const int eofSym = 0; /* pdt */
  	const int charSetSize = 256;
	const int maxT = 14;
	const int noSym = 14;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0, 14, 15, 12, 10,  0, 11,  0, 13,
	 17, 17, 17, 17, 17, 17, 17, 17, 17, 17,  0,  0,  1,  5,  3,  0,
	  0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
	 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,  6,  0,  8,  0,  0,
	  0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
	 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};


    public Buffer buffer; // scanner buffer
    
    Token t;          // current token
    char ch;          // current input character
    int pos;          // column number of current character
    int line;         // line number of current character
    int lineStart;    // start position of current line
    int oldEols;      // EOLs that appeared in a comment;
    BitArray ignore;  // set of characters to be ignored by the scanner

    Token tokens;     // list of tokens already peeked (first token is a dummy)
    Token pt;         // current peek token
    
    char[] tval = new char[128]; // text of current token
    int tlen;         // length of current token
    
    public Scanner (string fileName) {
      try {
        Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        buffer = new Buffer(stream, false);
        Init();
      } catch (IOException) {
        Console.WriteLine("--- Cannot open file {0}", fileName);
        System.Environment.Exit(1);
      }
    }
    
    public Scanner (Stream s) {
      buffer = new Buffer(s, true);
      Init();
    }
    
    void Init() {
      pos = -1; line = 1; lineStart = 0;
      oldEols = 0;
      NextCh();
      ignore = new BitArray(charSetSize+1);
      ignore[' '] = true;  // blanks are always white space
  		ignore[9] = true; ignore[10] = true; ignore[13] = true; 
      pt = tokens = new Token();  // first token is a dummy
    }
    
    void NextCh() {
      if (oldEols > 0) { ch = EOL; oldEols--; } 
      else {
        ch = (char)buffer.Read(); pos++;
        // replace isolated '\r' by '\n' in order to make
        // eol handling uniform across Windows, Unix and Mac
        if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
        if (ch == EOL) { line++; lineStart = pos + 1; }
      }
  
    }

    void AddCh() {
      if (tlen >= tval.Length) {
        char[] newBuf = new char[2 * tval.Length];
        Array.Copy(tval, 0, newBuf, 0, tval.Length);
        tval = newBuf;
      }
  		tval[tlen++] = ch;
      NextCh();
    }


  

    void CheckLiteral() {
  		switch (t.val) {
			default: break;
		}
    }

    Token NextToken() {
      while (ignore[ch]) NextCh();
  
      t = new Token();
      t.pos = pos; t.col = pos - lineStart + 1; t.line = line; 
      int state = start[ch];
      tlen = 0; AddCh();
      
      switch (state) {
        case -1: { t.kind = eofSym; break; } // NextCh already done
        case 0: { t.kind = noSym; break; }   // NextCh already done
  			case 1:
				if (ch == '=') {AddCh(); goto case 2;}
				else {t.kind = noSym; break;}
			case 2:
				{t.kind = 1; break;}
			case 3:
				if (ch == '=') {AddCh(); goto case 4;}
				else {t.kind = noSym; break;}
			case 4:
				{t.kind = 2; break;}
			case 5:
				{t.kind = 3; break;}
			case 6:
				if (ch == '=') {AddCh(); goto case 7;}
				else {t.kind = noSym; break;}
			case 7:
				{t.kind = 4; break;}
			case 8:
				if (ch == '=') {AddCh(); goto case 9;}
				else {t.kind = noSym; break;}
			case 9:
				{t.kind = 5; break;}
			case 10:
				{t.kind = 6; break;}
			case 11:
				{t.kind = 7; break;}
			case 12:
				{t.kind = 8; break;}
			case 13:
				{t.kind = 9; break;}
			case 14:
				{t.kind = 10; break;}
			case 15:
				{t.kind = 11; break;}
			case 16:
				if ((ch >= '-' && ch <= '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z')) {AddCh(); goto case 16;}
				else {t.kind = 12; break;}
			case 17:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 17;}
				else if (ch == '.') {AddCh(); goto case 18;}
				else {t.kind = 13; break;}
			case 18:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 18;}
				else if (ch == 'E') {AddCh(); goto case 19;}
				else {t.kind = 13; break;}
			case 19:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 21;}
				else if ((ch == '+' || ch == '-')) {AddCh(); goto case 20;}
				else {t.kind = noSym; break;}
			case 20:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 21;}
				else {t.kind = noSym; break;}
			case 21:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 21;}
				else {t.kind = 13; break;}

      }
      t.val = new String(tval, 0, tlen);
      return t;
    }
    
    // get the next token (possibly a token already seen during peeking)
    public Token Scan () {
      if (tokens.next == null) {
        return NextToken();
      } else {
        pt = tokens = tokens.next;
        return tokens;
      }
    }

    // peek for the next token, ignore pragmas
    public Token Peek () {
      if (pt.next == null) {
        do {
          pt = pt.next = NextToken();
        } while (pt.kind > maxT); // skip pragmas
      } else {
        do {
          pt = pt.next;
        } while (pt.kind > maxT);
      }
      return pt;
    }
    
    // make sure that peeking starts at the current scan position
    public void ResetPeek () { pt = tokens; }

  } // end Scanner
}

