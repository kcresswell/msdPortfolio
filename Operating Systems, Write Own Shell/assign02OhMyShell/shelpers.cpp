#include "shelpers.hpp"

/*
  text handling functions
 */

bool splitOnSymbol(std::vector<std::string>& words, int i, char c){
  if(words[i].size() < 2){ return false; }
  int pos;
  if((pos = words[i].find(c)) != std::string::npos){
	if(pos == 0){
	  //starts with symbol
	  words.insert(words.begin() + i + 1, words[i].substr(1, words[i].size() -1));
	  words[i] = words[i].substr(0,1);
	} else {
	  //symbol in middle or end
	  words.insert(words.begin() + i + 1, std::string{c});
	  std::string after = words[i].substr(pos + 1, words[i].size() - pos - 1);
	  if(!after.empty()){
		words.insert(words.begin() + i + 2, after);
	  }
	  words[i] = words[i].substr(0, pos);
	}
	return true;
  } else {
	return false;
  }

}

std::vector<std::string> tokenize(const std::string& s){

  std::vector<std::string> ret;
  int pos = 0;
  int space;
  //split on spaces
  while((space = s.find(' ', pos)) != std::string::npos){
	std::string word = s.substr(pos, space - pos);
	if(!word.empty()){
	  ret.push_back(word);
	}
	pos = space + 1;
  }

  std::string lastWord = s.substr(pos, s.size() - pos);
  if(!lastWord.empty()){
	ret.push_back(lastWord);
  }

  for(int i = 0; i < ret.size(); ++i){
	for(auto c : {'&', '<', '>', '|'}){
	  if(splitOnSymbol(ret, i, c)){
		--i;
		break;
	  }
	}
  }
  
  return ret;
  
}


std::ostream& operator<<(std::ostream& outs, const Command& c){
  outs << c.exec << " argv: ";
  for(const auto& arg : c.argv){ if(arg) {outs << arg << ' ';}}
  outs << "fds: " << c.fdStdin << ' ' << c.fdStdout << ' ' << (c.background ? "background" : "");
  return outs;
}

//returns an empty vector on error
/*

  You'll need to fill in a few gaps in this function and add appropriate error handling
  at the end.

 */
std::vector<Command> getCommands(const std::vector<std::string>& tokens) {
  std::vector<Command> ret(std::count(tokens.begin(), tokens.end(), "|") + 1);  //1 + num |'s commands

  int first = 0;
  int last = std::find(tokens.begin(), tokens.end(), "|") - tokens.begin();
  bool error = false;
  for(int i = 0; i < ret.size(); ++i) {
	if((tokens[first] == "&") || (tokens[first] == "<") ||
		(tokens[first] == ">") || (tokens[first] == "|")) {
	  error = true;
	  break;
	}

	ret[i].exec = tokens[first];
	ret[i].argv.push_back(tokens[first].c_str()); //argv0 = program name
	std::cout << "exec start: " << ret[i].exec << std::endl;
	ret[i].fdStdin = 0;
	ret[i].fdStdout = 1;
	ret[i].background = false;
	
      //I/O redirection
      /*
       Eventually you'll need to fill this in to support I/O Redirection
       Note, that only the FIRST command can take input redirection
       (all others get input from a pipe)
       Only the LAST command can have output redirection!
       */
	for(int j = first + 1; j < last; ++j) {
	  if(tokens[j] == ">" || tokens[j] == "<" ) {
          if(tokens[j] == ">") {
              //              //echo hello world > someFile
              ret[i].fdStdout = open(tokens[j+1].c_str(), O_CREAT | O_WRONLY); //write only
          } else {
              ret[i].fdStdin = open(tokens[j+1].c_str(), O_RDONLY); //read only
          }
          j++;
      } else if(tokens[j] == "&") {
		//Fill this in if you choose to do the optional "background command" part
        //Part 5 Background Process Logic
          ret[i].background = true;
	  } else {
		//otherwise this is a normal command line argument!
		ret[i].argv.push_back(tokens[j].c_str());
	  }
	  
	}
	if(i > 0) {
	  /* there are multiple commands.  Open open a pipe and
		 Connect the ends to the fds for the commands!
	  */
        int fds[2];
        if(pipe(fds) < 0) {
            perror("Error with opening the pipe. :(");
        }
        ret[i].fdStdin = fds[0]; //1 to read so just i
        ret[i -1].fdStdout = fds[1];  //0 to write so i - 1
        
	}
	//exec wants argv to have a nullptr at the end!
	ret[i].argv.push_back(nullptr);

	//find the next pipe character
	first = last + 1;
	if(first < tokens.size()) {
	  last = std::find(tokens.begin() + first, tokens.end(), "|") - tokens.begin();
	}
  }

//  if(error) {
//    //close any file descriptors you opened in this function!
//
//    assert(false);
//
//  }
  
  return ret;
}
