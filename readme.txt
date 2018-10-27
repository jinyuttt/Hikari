                 HiKari使用说明
程序初始化
  使用配置类
   HikariConfig hikariConfig = new HikariConfig();
            hikariConfig.DBType = "PostgreSQL";
            hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            hikariConfig.DriverDir = "DBDrivers";
            hikariConfig.DriverDLL = "XXXX.dll";
            hikariConfig.DBTypeXml = "DBType.xml";

            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);

直接使用
  HikariDataSource hikariDataSource = new HikariDataSource();
            hikariDataSource.DBType = "PostgreSQL";
            hikariDataSource.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            hikariDataSource.DriverDir = "DBDrivers";
            hikariDataSource.DriverDLL = "XXXX.dll";
            hikariDataSource.DBTypeXml = "DBType.xml";

配置文件
HikariConfig hikariConfig = new HikariConfig();
hikariConfig.LoadConfig("Hikari.txt");
//也可以使用
HikariDataSource hikariDataSource = new HikariDataSource();
            hikariDataSource.LoadConfig("Hikari.txt");

使用
hikariDataSource.GetConnection();

说明：程序根据配置，会调用驱动DLL.
驱动dll查找的路径：DriverDir+ DriverDLL，所以一定要配置好。
另外一种是，如果你没有配置，而是配置了DBType.程序将会根据DBType去查找dll名称，自动去配置DriverDLL；
DBType程序中写死了4种，见附录。
另外会读取DBType.xml文件，获取全局配置信息。该配置文件具体信息允许你在程序中设置HikariConfig或者HikariDataSource的DBTypeXml属性。


源码简说
HikariDataSource 对外提供连接
HikariConfig 对外配置
HikariPool 管理操作集合，连接来源
PoolBase 操作驱动连接，是HikariPool父类


附录
数据库	Dll名称	说明
Oracle	Oracle.ManagedDataAccess	以前不是这个dll
MySql	MySql.Data	
SqlServer	System	
PostgreSQL	Npgsql	

