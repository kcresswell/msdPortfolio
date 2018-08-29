//
//  main.cpp
//  assign02OhMyShell
//
//  Created by Kayla Cresswell on 1/29/18.
//  Copyright © 2018 Kayla Cresswell. All rights reserved.
//

#include "shelpers.hpp"


int main(int argc, const char * argv[]) {
    // ------------ PART 1 ------------ //
    pid_t pid, wpid;
    std::string input;
    std::vector<Command> re; //list of commands.
    std::vector<std::string> tokens; //ls etc.
    std::vector<pid_t> backgroundChildren;
    std::cout << "$ ";
    int status;
    while(std::getline(std::cin, input)) {
        tokens = tokenize(input);
        re = getCommands(tokens);
        
        //added to handle multiple commnands
        for(int i = 0; i < re.size(); i++) {
            //cd code - Part 4
            if(strcmp(re[i].argv[0], "cd") == 0) {
                //change to home directory if no parameters are provided
                if(re[i].argv[1] == NULL) {
                    chdir(getenv("HOME"));
                } else if(chdir(re[i].argv[1])< 0) {
                    //change directories to specified location
                    perror("cd failed");
                }
                //skip the for process in pid
                continue;
            }
            
            //avoid zombie children
            while(pid_t zombieKiddos = waitpid(-1, &status, WNOHANG) > 0) {
                //delete the zombies from the vector
                backgroundChildren.erase(std::remove(backgroundChildren.begin(), backgroundChildren.end(), zombieKiddos), backgroundChildren.end());
            }
            
            std::vector<pid_t> foregroundChildren;
            
            pid = fork();
            
            if(re[i].background) {
                backgroundChildren.push_back(pid);
            } else {
                foregroundChildren.push_back(pid);
            }
            
            if(pid == 0) {
                if(re[i].fdStdin != 0) {
                    //read pipe has been triggered
                    dup2(re[i].fdStdin, 0);
                }
                
                if(re[i].fdStdout != 1) {
                    //write to pipe has been triggered
                    dup2(re[i].fdStdout, 1);
                }
                
                //in child process
                if(execvp(re[i].argv[0], const_cast<char* const*>(re[i].argv.data())) ==  -1) { //program name, path == -1 is an error
                    perror("exec error");
                }
                //exit child process
                exit(0);
            } else if(pid < 0) {
                perror("fork error");
            } else {
                //in parent process
                if(re[i].fdStdin != 0) {
                    close(re[i].fdStdin);
                }
                if(re[i].fdStdout != 1) {
                    close(re[i].fdStdout);
                }
                //part 5 background process code:
                if(re[i].background == true){
                    //current process should wait
                    printf("Process Created With PID: %d\n", pid);
                } else {
                    //wait for child process - avoid zombie child 
                    wpid = waitpid(pid, &status, WNOHANG);
                    if(wpid == -1){
                        perror("wpid failed");
                    }
                }
            }
        }
        std::cout << "$ ";
    }

    return 0;
}
