# WindowsServicePluginJobDiscovery
Creating to reproduce issue with using Hangfire jobs in a plugin model of windows service
* https://github.com/HangfireIO/Hangfire/issues/470

For WindowsService.Hangfire and WindowsService.Quartz, to test execution you must copy files from Cyrus.MicroServices\Plugins\Cyrus.ClientMergeEmails\bin\Debug to Cyrus.MicroServices\Cyrus.MicroServices\PluginsTemp after the app has started.
