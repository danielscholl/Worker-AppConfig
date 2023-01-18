# Instructions

1. Deploy a new instance of Azure App Configuration.

2. Add in 2 Configurations
  - Worker:Set

3. Create a new `.env` file

```bash
APP_CONFIG_ENDPOINT=https://<your_endpoint>.azconfig.io
AZURE_TENANT_ID=<your_tenant_id>
AZURE_CLIENT_ID=<your_client_id>
AZURE_CLIENT_SECRET=<your_client_secret>
```

4. Grant the service principal used in Step 3 access to the App Configuration instance.
  - App Configuration Data Owner
