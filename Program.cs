using Microsoft.Playwright;
using PlaywrightDemo;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = false });

var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = @"N:\.NET CORE\PlaywrightDemo\auth.json" });

var page = await context.NewPageAsync();
await page.AddInitScriptAsync(scriptPath: @"N:\.NET CORE\PlaywrightDemo\stealth.min.js");
await page.GotoAsync("https://wap.showstart.com/pages/passport/login/login?redirect=%2Fpages%2FmyHome%2FmyHome");

var helper = new Helper(page, "0fc7521c6836ac036a39b1dabace907e", "182990", false, null, "1", 1);

var payBtn = await helper.LoadTicketPage();

await helper.BuyTicket(payBtn);

await context.StorageStateAsync(new BrowserContextStorageStateOptions() { Path= @"N:\.NET CORE\PlaywrightDemo\auth.json" });
await context.CloseAsync();
await browser.CloseAsync();