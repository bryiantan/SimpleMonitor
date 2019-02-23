# Poor Man Web Monitoring tools
##### Simple IIS Log Viewer, Files Compare, Ban IP address

Like many other bloggers or website owner out there, most of us host our website on a shared server instead of dedicated server hosting due to the cost. The hosting company shall not be name here. Based on my own experiences, once in a while, some malicious folders and files / malware were being injected into the website. Every time I submit a ticket about it, I will get a response like “your computer was compromised”, “you’re using outdated software”, “you need to change your password”, etc. All kind of nonsense, and the hosting company never took responsibility or doing their due diligence. Some hosting company even offer to clean up the vulnerability and monitoring EACH site on the hosting for a yearly subscription. Imagine if you more than one website, the cost will quickly add up due to someone else negligence. 

Since I refused to pay some company every year to monitor all my websites, why not build my own? The title said it all, “Poor Man Web Monitoring tools”, if you’re poor, then you need to work hard and put together all the available free tools yourself. It might look like a Frankenstein tool now, but I’m positive it will look better once we pour some more thought into it. 

Shown below is the brief description of some of the components in the solution. In this article, I’ll not go into very detail on how the solution were being implemented as I believed some more works are needed to optimized it. But I will share, what each component will do and how to set it up.

1.	DownloadIISLog 
    *	Download IIS Log files thru FTP (WinSCP)
    *	Insert the log files data into the SQL Database using Log Parser
2.	SimpleMonitor.Web
    *	Web UI to view the IIS Log data thru jqGrid
    *	Option to mark certain Ip Addresses "Banned"
3.	SimpleMonitor.BlockIpAddress
    *	Example of HTTP Module to block banned Ip address from accessing the Website
4.	SimpleMonitor.ScanFile
    *	Download latest files
    *	Compare Baseline vs latest files using WinMerge
    *	Send email notification of the results


**Require software:**
Download and install the following software.

1.	WinMerge v2.15.2 (https://sourceforge.net/projects/winmerge/files/alpha/2.15.2/) 
2.	Log parser v2.2 (https://www.microsoft.com/en-us/download/details.aspx?id=24659) 


**View the IISLog through jqGrid**
![alt tag](https://blog.ysatech.com/images/simple_monitor/jqgrid_iislog.PNG)
**Block bad IP addresses**
![alt tag](https://blog.ysatech.com/images/simple_monitor/blocking_ip.PNG)
**Compare files**

![alt tag](https://blog.ysatech.com/images/simple_monitor/compare_result_2.PNG)
