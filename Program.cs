using Microsoft.Playwright;
using PlaywrightDemo;

//加载配置
var config = AppConfig.Load();

var authPath = "auth.json";
AppConfig.CheckAuthFile(authPath);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = false });
var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = authPath });

var page = await context.NewPageAsync();
await page.AddInitScriptAsync(scriptPath: @"stealth.min.js");

var helper = new Helper(page, config);
var isLogin = await helper.CheckLogin();
if (!isLogin)
{
    return;
}

await helper.PurchaseLoop();

await context.StorageStateAsync(new BrowserContextStorageStateOptions() { Path = authPath });
await context.CloseAsync();
await browser.CloseAsync();