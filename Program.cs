using Microsoft.Playwright;
using PlaywrightDemo;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = false });

var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = @"N:\.NET CORE\PlaywrightDemo\auth.json" });

var page = await context.NewPageAsync();
await page.AddInitScriptAsync(scriptPath: @"N:\.NET CORE\PlaywrightDemo\stealth.min.js");
var helper = new Helper(page, "0fc7521c6836ac036a39b1dabace907e", "182990", false, null, "1", 1);

var isLogin = await helper.Login();

if (!isLogin)
{
    return;
}

var payBtn = await helper.GetPayBtn();

if (payBtn == null)
{
    Console.WriteLine("未获取到支付按钮");
    return;
}

await helper.BuyTicket(payBtn);

await context.StorageStateAsync(new BrowserContextStorageStateOptions() { Path= @"N:\.NET CORE\PlaywrightDemo\auth.json" });
await context.CloseAsync();
await browser.CloseAsync();