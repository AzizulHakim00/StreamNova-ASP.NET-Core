# Deploy StreamNova for free on MonsterASP.NET

StreamNova is prepared for the MonsterASP.NET free plan using ASP.NET Core 10, IIS in-process hosting, a free subdomain, and GitHub Actions.

## What the free plan provides

- One ASP.NET website
- A free `runasp.net` or `tryasp.net` subdomain
- .NET 10 hosting
- 5 GB website storage
- 256 MB RAM
- One database with 1 GB storage
- No credit card requirement

The free plan is designed for learning, demonstrations, portfolios, and small development projects. It has limited traffic and no uptime guarantee.

## First-time hosting setup

1. Create a free MonsterASP.NET account.
2. In its control panel, create one free ASP.NET website.
3. Choose the .NET 10 runtime.
4. Open the website's **Deploy (FTP/WebDeploy/Git)** page.
5. Enable **WebDeploy**.
6. Copy the website name, server URL, username, and password shown by the control panel.

Do not place the WebDeploy password in source code or commit it to Git.

## Connect GitHub Actions

Open this repository on GitHub and go to:

`Settings -> Secrets and variables -> Actions -> New repository secret`

Create these four secrets using the values shown in the MonsterASP.NET control panel:

| GitHub secret | Example format |
|---|---|
| `MONSTER_WEBSITE_NAME` | `site123456` |
| `MONSTER_SERVER_COMPUTER_NAME` | `https://site123456.siteasp.net:8172` |
| `MONSTER_SERVER_USERNAME` | `site123456` |
| `MONSTER_SERVER_PASSWORD` | WebDeploy password |

Then open:

`Actions -> Build and deploy StreamNova to MonsterASP.NET -> Run workflow`

The workflow will restore, build, publish for Windows IIS, create a ZIP artifact, and deploy the published files through WebDeploy.

## Manual ZIP deployment

The workflow always creates an artifact called:

`streamnova-monsterasp-package`

To deploy manually:

1. Open the latest workflow run in GitHub Actions.
2. Download the artifact.
3. In MonsterASP.NET, open the website File Manager.
4. Upload the ZIP file.
5. Extract it directly into the website `/wwwroot` directory.
6. Restart the website from the hosting control panel.

## Runtime data

StreamNova stores its runtime state in:

`App_Data/streamnova.json`

This includes accounts, movie data, watchlists, progress, reviews, subscriptions, notifications, and support tickets. Back up this file before replacing the whole website directory.

The deployment package does not include `streamnova.json`, so the application creates it automatically on first start.

## Demo accounts

Administrator:

- Email: `admin@streamnova.local`
- Password: `Admin@123`

Customer:

- Email: `user@streamnova.local`
- Password: `User@123`

Change the administrator password after confirming the deployment.

## Health check

After deployment, open:

`https://YOUR-SUBDOMAIN/health`

A successful response confirms the ASP.NET application started correctly.

## Troubleshooting

### HTTP 500.30 or startup failure

- Confirm the website runtime is .NET 10.
- Confirm the application pool uses ASP.NET Core in-process hosting.
- Restart the website from the hosting control panel.
- Check the hosting logs.

### Static images or CSS do not load

Confirm the entire published ZIP was extracted into `/wwwroot`, including the application `wwwroot` directory.

### Data is reset after redeployment

Restore a backup of `App_Data/streamnova.json`. Avoid deleting the destination `App_Data` directory during manual updates.

### GitHub deployment is skipped

Confirm all four `MONSTER_*` repository secrets exist and rerun the workflow.
