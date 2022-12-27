using Microsoft.Playwright;
using OpenCvSharp;

namespace PlaywrightDemo;

public class Helper
{
    public string ActivityId;
    public string TicketId;
    public string TicketNum;
    public DateTime? StartTime;
    public bool NeedSelect;
    public int SelectNum;
    public IPage Page;

    public Helper(IPage page,
                 AppConfig appConfig)
    {
        this.Page = page;
        ActivityId = appConfig.ActivityId;
        TicketId = appConfig.TicketId;
        TicketNum = appConfig.TicketNum;
        SelectNum = appConfig.SelectNum;
        StartTime = appConfig.StartTime;
        NeedSelect = appConfig.NeedSelect;
    }

    public async Task<bool> CheckLogin()
    {
        //检查cookie是否过期
        await Page.GotoAsync("https://wap.showstart.com/pages/myHome/myHome",
            new() { WaitUntil = WaitUntilState.NetworkIdle });

        try
        {
            //是否有登录按钮
            var loginBtn = await Page.WaitForSelectorAsync(".login-btn", new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            //未登录 跳转登录页面
            if (loginBtn != null)
            {
                //滑块验证码登录
                return await Login();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("登录成功");
            return true;
        }

        return true;
    }

    public async Task<bool> Login()
    {
        await Page.GotoAsync("https://wap.showstart.com/pages/passport/login/login?redirect=%252Fpages%252FmyHome%252FmyHome",
    new() { WaitUntil = WaitUntilState.NetworkIdle });

        await Page.WaitForTimeoutAsync(1000 * 50);

        if (await Page.TitleAsync() == "我的")
        {
            return true;
        }

        return false;
    }

    public async Task<IElementHandle?> GetPayBtn()
    {
        var confirmUrl = $@"https://wap.showstart.com/pages/order/activity/confirm/confirm?sequence=" +
   $@"{ActivityId}&ticketId={TicketId}&ticketNum={TicketNum}&ioswx=1&terminal=app&from=singlemessage&isappinstalled=0";

        IElementHandle? payBtn;
        var flashCount = 0; // 刷新次数
        while (true)
        {
            //超过20次循环未获取按钮返回
            if (flashCount >= 10)
            {
                return null;
            }

            await Page.GotoAsync(confirmUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
            try
            {
                payBtn = await Page.WaitForSelectorAsync(".payBtn", new() { State = WaitForSelectorState.Visible, Strict = true, Timeout = 5000 });
            }
            catch (Exception)
            {
                Console.WriteLine("获取支付按钮超时");
                continue;
            }
            var text = payBtn == null ? "" : await payBtn.TextContentAsync();
            if (text != null && text.Contains("立即支付"))
            {
                Console.WriteLine("获取支付按钮成功！");
                break;
            }

            flashCount++;
        }

        return payBtn;
    }

    public async Task BuyTicket(IElementHandle payBtn)
    {
        Console.WriteLine($"是否需要选择观演人: {NeedSelect}, 如果需要, 选择数量: {SelectNum}");
        if (NeedSelect)
        {
            //选择观演人
        }

        Console.WriteLine("开始抢票");
        while (true)
        {
            try
            {
                await payBtn.ClickAsync(new() { ClickCount = 3 });
            }
            catch (Exception e)
            {
                Console.WriteLine("点击支付按钮发生异常，可能是已经抢票成功, 请查看手机 但是先不要退出");
                break;
            }

            await Page.WaitForTimeoutAsync(300);
        }

    }

    public async Task SelectPerson()
    {
        try
        {
            var selectBtn = await Page.WaitForSelectorAsync(".link-item>.rr>.tips", new() { State = WaitForSelectorState.Visible, Timeout = 200 });
            await selectBtn.ClickAsync();

            for (int i = 0; i < SelectNum; i++)
            {
                var checkbox = await Page.WaitForSelectorAsync($".uni-scroll-view-content > uni-checkbox-group > uni-label:nth-child({i + 1})", new() { Timeout = 200 });
                await checkbox.ClickAsync();
            }

            var confirmBtn = await Page.WaitForSelectorAsync(".pop-box>.pop-head>uni-view:nth-child(2)", new() { State = WaitForSelectorState.Visible, Timeout = 200 });
            confirmBtn?.ClickAsync();
        }
        catch (Exception)
        {
            Console.WriteLine("未获取到选择观演人按钮");
            await Page.ReloadAsync();
        }
    }

    public async Task PurchaseLoop(int loopCount = 10)
    {
        for (int i = 0; i < loopCount; i++)
        {
            if (NeedSelect)
            {
                await SelectPerson();
            }

            var payBtn = await GetPayBtn();

            if (payBtn == null)
            {
                Console.WriteLine("未获取到支付按钮");
                continue;
            }

            await BuyTicket(payBtn);
            Thread.Sleep(300); //休眠300毫秒避免过快刷新被封禁
        }

    }

    public int GetDistance(string sliderPath, string backgroudPath)
    {
        using var slider = Cv2.ImRead(sliderPath, ImreadModes.Grayscale);
        using var backgroudImg = Cv2.ImRead(backgroudPath, ImreadModes.Grayscale);

        var img1 = TranCanny(slider);
        var img2 = TranCanny(backgroudImg);
        var res = new Mat();
        Cv2.MatchTemplate(img1, img2, res, TemplateMatchModes.CCoeffNormed);

        double minVal,maxVal;
        Point minLoc, maxLoc;
        Cv2.MinMaxLoc(res, out minVal, out maxVal, out minLoc, out maxLoc);
        var topLeft = maxLoc.X;

        return topLeft * 278 / 360; 
    }

    public Mat TranCanny(Mat img)
    {
        Cv2.GaussianBlur(img, img, ksize: new Size(3, 3), sigmaX: 0);
        Cv2.Canny(img, img, 50, 150);
        return img;
    }
}