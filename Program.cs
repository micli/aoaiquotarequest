using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Newtonsoft.Json;
using OpenQA.Selenium.Interactions;

namespace AOAIQuota
{
    public class Program
    {
        private static IWebDriver _edgeDriver = null!;

        private static IWebElement GetElementById(IWebDriver driver, string elementId)
        {
            // Find the element by ID
            var element = driver.FindElement(By.Id(elementId));
            return element;
        }

        private static IWebElement GetElementByClassName(IWebDriver driver, string className)
        {
            // Find the element by class name
            var element = driver.FindElement(By.ClassName(className));
            return element;
        }
        private static ReadOnlyCollection<IWebElement> GetAllInputElements(IWebDriver driver)
        {
            // Find all input elements on the page
            var inputElements = driver.FindElements(By.TagName("input"));
            return inputElements;
        }

        private static ReadOnlyCollection<IWebElement> GetAllSpanElements(IWebDriver driver)
        {
            // Find all input elements on the page
            var inputElements = driver.FindElements(By.TagName("span"));
            return inputElements;
        }

        private static ReadOnlyCollection<IWebElement> GetAllButtonElements(IWebDriver driver)
        {
            // Find all button elements on the page
            var buttonElements = driver.FindElements(By.TagName("button"));
            return buttonElements;
        }
        private static ReadOnlyCollection<IWebElement> GetAlTextAreaElements(IWebDriver driver)
        {
            // Find all link elements on the page
            var linkElements = driver.FindElements(By.TagName("textarea"));
            return linkElements;
        }

        private static ReadOnlyCollection<IWebElement> GetAllOptionsElements(IWebDriver driver)
        {
            // Find all link elements on the page
            var linkElements = driver.FindElements(By.ClassName("text-format-content"));
            return linkElements;
        }

        private static ReadOnlyCollection<IWebElement> GetAllInputElementByCss(IWebDriver driver)
        {
            // Find all link elements on the page
            var linkElements = driver.FindElements(By.ClassName("office-form-question-textbox"));
            return linkElements;
        }

        private static IWebElement RetrieveElementByValue(ReadOnlyCollection<IWebElement> inputList, string elementValue)
        {
            foreach (var input in inputList)
            {
                try
                {
                    var value = input.GetAttribute("value");
                    if (value == elementValue)
                    {
                        return input;
                    }
                }
                catch (Exception)
                {
                    // 忽略异常，继续处理下一个元素
                    continue;
                }
            }
            throw new Exception("未找到具有指定值的输入控件");
        }

        private static IWebElement RetrieveElementByText(ReadOnlyCollection<IWebElement> inputList, string elementValue)
        {
            foreach (var input in inputList)
            {
                try
                {
                    var value = input.Text.ToLower();
                    if (value == elementValue.ToLower())
                    {
                        return input;
                    }
                }
                catch (Exception)
                {
                    // 忽略异常，继续处理下一个元素
                    continue;
                }
            }
            throw new Exception("未找到具有指定值的输入控件");
        }

        private static IWebElement GetDropdownElement(IWebDriver driver)
        {
            // Find the dropdown element by ID
            var dropdown = driver.FindElement(By.Id("t37_placeholder"));
            if (!dropdown.Displayed)
            {
                dropdown = driver.FindElement(By.Id("t39_placeholder"));
            }
            if (!dropdown.Displayed)
            {
                dropdown = driver.FindElement(By.Id("t41_placeholder"));
            }
            if (!dropdown.Displayed)
            {
                dropdown = driver.FindElement(By.Id("t43_placeholder"));
            }           
            return dropdown;
        }

        private static void FillQuota(ReadOnlyCollection<IWebElement> inputElements, string quota)
        {
            // 填充配额的逻辑
            foreach (var input in inputElements)
            {
                try
                {
                    var placeholder = input.GetAttribute("placeholder") ?? string.Empty;
                    var value = input.GetAttribute("value") ?? string.Empty;
                    if (input.Displayed && placeholder.Contains("请按指定格式输入值") && value == string.Empty)
                    {
                        input.SendKeys(quota);
                        continue; // 找到第一个空的输入框后退出循环
                    }
                }
                catch (Exception)
                {
                    // 忽略异常，继续处理下一个元素
                    continue;
                }
            }
        }

        
        private static void Initialize()
        {
            var edgeOptions = new EdgeOptions();
            edgeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
            _edgeDriver = new EdgeDriver(edgeOptions);
        }
        private static void LoadPage()
        {
            // 使用 JavaScript 打开一个新标签页
            ((IJavaScriptExecutor)_edgeDriver).ExecuteScript("window.open();");

            // 切换到新标签页
            var tabs = _edgeDriver.WindowHandles;
            _edgeDriver.SwitchTo().Window(tabs[tabs.Count - 1]);
            _edgeDriver.Navigate().GoToUrl("https://aka.ms/oai/stuquotarequest");
            Thread.Sleep(2000); // 等待页面加载完成
        }

        private static void stop()
        {
            if (_edgeDriver != null)
            {
                _edgeDriver.Quit();
                _edgeDriver.Dispose();
            }
        }

        private static void FillForm(QuotaInfo quotaInfo, string subid, string regionName)
        {
            // 填充表单
            var inputElements = GetAllInputElementByCss(_edgeDriver);
            var buttonElements = GetAllButtonElements(_edgeDriver);
            var textAreaElements = GetAlTextAreaElements(_edgeDriver);
            var spanElements = GetAllSpanElements(_edgeDriver);

            string[] inputValues = new string[]
            {
                quotaInfo.FirstName,
                quotaInfo.LastName,
                quotaInfo.CompanyEmail,
                quotaInfo.CompanyName,
                quotaInfo.CompanyAddress,
                quotaInfo.CompanyCity,
                quotaInfo.CompanyPostalCode,
                quotaInfo.CompanyCountry,
                subid
            };
            for (int i = 0; i < 9; i++)
            {
                try
                {
                    var value = inputElements[i].GetAttribute("value") ?? string.Empty;
                    if (value == string.Empty)
                    {
                        inputElements[i].SendKeys(inputValues[i]);
                        continue;
                    }
                }
                catch (Exception)
                {
                    // 忽略异常，继续处理下一个元素
                    continue;
                }
            }

            // Business justification
            if(string.Empty == textAreaElements[0].GetAttribute("value"))
            {
                textAreaElements[0].SendKeys("Business needs, already reaches quota limitation, needs to increase.");
            }

            // Select deployment type
            var deploymentType = RetrieveElementByText(spanElements, quotaInfo.Deployment);
            if (deploymentType != null)
            {
                deploymentType.Click();
            }
            else
            {
                throw new Exception("未找到指定的部署类型");
            }

            // region
            new Actions(_edgeDriver).SendKeys(Keys.PageDown).Perform();
            var region = RetrieveElementByText(spanElements, regionName);
            region.Click(); // 点击选择区域

            // Model
            new Actions(_edgeDriver).SendKeys(Keys.PageDown).Perform();
            Thread.Sleep(500);
            if (quotaInfo.Deployment != "Global Provisioned" && quotaInfo.Deployment != "Data Zone Provisioned")
            {
                var dropdown = GetDropdownElement(_edgeDriver);
                dropdown.Click(); // 点击下拉框
                Thread.Sleep(1000); // 等待下拉框加载完成
                var optionsElements = GetAllOptionsElements(_edgeDriver);
                var model = RetrieveElementByText(optionsElements, quotaInfo.Model);
                model.Click(); // 点击选择区域
            }

            // quota
            var textInputElements = GetAllInputElementByCss(_edgeDriver);
            FillQuota(textInputElements, quotaInfo.AskQuota);

            // 提交表单
            var submitButton = GetAllButtonElements(_edgeDriver)[0];
            try
            {
                submitButton.Click();
            }
            catch (ElementClickInterceptedException ex)
            {
                System.Console.WriteLine($"点击提交按钮时发生错误: {ex.Message}");
            }
            System.Console.WriteLine($"{quotaInfo.Deployment}\t部署在{regionName}\t申请配额:{quotaInfo.AskQuota}");
        }
        public static void Main(string[] args)
        {
            // 检查是否提供了参数
            if (args.Length == 0)
            {
                Console.WriteLine("错误: 请提供一个 JSON 文件作为参数。");
                Environment.Exit(1);
            }

            // 检查文件是否为 JSON 文件
            var filePath = args[0];
            if (!File.Exists(filePath) || Path.GetExtension(filePath).ToLower() != ".json")
            {
                Console.WriteLine("错误: 提供的文件不存在或不是 JSON 文件。");
                Environment.Exit(1);
            }

            Console.WriteLine($"成功加载文件: {filePath}");
            string jsonContent = File.ReadAllText(filePath);
            QuotaInfo quotaInfo = JsonConvert.DeserializeObject<QuotaInfo>(jsonContent) ?? throw new Exception("JSON content is null.");
            // 调用加载页面的方法
            Initialize();

            for (int i = 0; i < quotaInfo.SubscriptionIdList.Length; i++)
            {
                var subid = quotaInfo.SubscriptionIdList[i];
                Console.WriteLine($"正在处理订阅 ID: {subid}");
                for(int j = 0; j < quotaInfo.RegionList.Length; j++)
                {
                    var region = quotaInfo.RegionList[j];
                    try
                    {
                        LoadPage();
                        FillForm(quotaInfo, subid, region);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"处理订阅 ID: {subid} 时发生错误: {ex.Message}");
                        // 忽略异常，继续尝试
                        Thread.Sleep(2000); // 等待页面加载完成
                    }
                }
            }
            // 关闭浏览器
            stop();
        }
    }

}
