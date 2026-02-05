## 🌡️ Getting Started 🌡️

*Disclamer: Please review the requirements.txt for all librarys and imports necessary for running this application*\
Follow these steps to get up and running quickly:\

1. **Clone the Project using ssh (for security of repo and easy authentication later)**\
   In your bash terminal:\
   git clone git@github.com:gelab9/LittleBlue.git\

   *Depending on admin rights and IT blockage, you will highley likely need to generate this key using powershell as adminstrator*\
   *Note: generate this key as follows and then go back into vs and finish cloning the repository*\

   Generate ssh key pair:\
   ssh-keygen -t ed25519 -C "your_email@example.com"\

   Once key has generated, log into github using your browser, go to ssh keys section in settings, and paste your generated contents as a new key.\

   *Finish cloning repository in vs code in your bash terminal*\

   Paste this message to comfirm your ssh connection:\
   ssh -T git@github.com\

   *You should see a message comfirming your authenticated*\

   Start the agent process:\
   eval "$(ssh-agent -s)"\

   Then proceed with adding your key in the SSH agent:\
   ssh-add ~/.ssh/id_ed25519\

   *Begin cloning the repository*\
   *Go to your repository, click code, and copy the ssh link*\

   Then paste this command in your bash terminal to clone:\
   git clone git@github.com:username/repository.git\

## Branches

Within our repository, there are two branches we use, main and develop. Main consists the BigBlue code
that was recreated for new sources and then develop consists of the code for the reformated Temperature Rise
application. Below, is how to navigate both branches, updating code, and what to do if you ever need to force push and pull.

**Fetching develop from the remote**\
*This just means that if you just cloned it, you will not have develop locally, so you will need to get the branch from github*\
Make sure your branch is clean:\
git branch\
git status\

Fetch and checkout:\
git fetch origin\
git checkout -b develop origin/develop\

**Checking out the develop branch**\
Check and see what branch you are in:\
git branch\
git status\

Switch to the develop branch:\
git switch develop\

Checkout the branch:\
git checkout develop\

**Switching back to main**\
Make sure your branch is clean:\
git branch\
git status\

*This is for if you don't have main locally yet*\
git fetch origin\

Switch to the main branch:\
git switch main\

**In the event that you need to force push and pull**\
*This is for when you need to overwrite your remote changes to your current local files*\
git push --force origin main\

or\

git push --force origin <branch-name>\

To reset those changes:\
git reset --hard <commit-hash>\
git push --force origin <branch-name>\

To pull those new forced files:\
Make sure your actually in the repo:\
git status\
*You should see 'On Branch develop'*\

Confirm the remote:\
git remote -v\

*You should see*
origin  git@github.com:your-org/your-project.git (fetch)\
origin  git@github.com:your-org/your-project.git (push)\

Fetch everything fresh from the remote:\
git fetch origin\

Checkout the develop branch:\
git checkout develop\

or if you don't have it locally\

git checkout -b develop origin/develop\

Reset your local branch to match the remote branch:\
git reset --hard origin/develop\

Verify:\
git status\

*and you should see*\
On branch develop\
Your branch is up to date with 'origin/develop'.\
nothing to commit, working tree clean\

## Testing and Development
**To connect the dotnet application and check connections**\
STEP 1. When connecting to COM Port use:\
curl.exe -i -X POST "http://127.0.0.1:5055/daq/connect" -H "Content-Type: application/json" --data-binary "@connect.json"\
Step 2. Immediately call daq idn to talk:\
curl.exe -i "http://127.0.0.1:5055/daq/idn"\

These following lines of code are for checking the daq34970A instrument, getting a serious connection returning:\
Manufacturer , Serial, and FW (Firmware version), idn stands for identification truncated:\
*What you should see*\
{"idn":"HEWLETT-PACKARD,34970A,0,13-2-2"}\
Lines to indentify 34970A (Agilent):\
curl.exe -i -X POST "http://127.0.0.1:5055/daq/connect"\
  -H "Content-Type: application/json"\
  --data-binary "@connect.json"\

curl.exe -i "http://127.0.0.1:5055/daq/idn"\

