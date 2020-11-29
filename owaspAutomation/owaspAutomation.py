from zapv2 import ZAPv2

import time
from pprint import pprint
from zapv2 import ZAPv2

target = 'https://orf.at/'
apikey = '8g0mshc4eva8ssubp291h59df7' 

zap = ZAPv2(apikey=apikey, proxies={'http': 'http://127.0.0.1:8090', 'https': 'http://127.0.0.1:8090'})

print('Accessing target {}'.format(target))
zap.urlopen(target)
# Give the sites tree a chance to get updated
time.sleep(2)

print('Spidering target {}'.format(target))
scanid = zap.spider.scan(target)
# Give the Spider a chance to start
time.sleep(2)
while (int(zap.spider.status(scanid)) < 100):
    # Loop until the spider has finished
    print('Spider progress %: {}'.format(zap.spider.status(scanid)))
    time.sleep(2)

print ('Spider completed')

while (int(zap.pscan.records_to_scan) > 0):
      print ('Records to passive scan : {}'.format(zap.pscan.records_to_scan))
      time.sleep(2)

print ('Passive Scan completed')

print ('Active Scanning target {}'.format(target))
scanid = zap.ascan.scan(target)
while (int(zap.ascan.status(scanid)) < 100):
    print ('Scan progress %: {}'.format(zap.ascan.status(scanid)))
    time.sleep(5)

print ('Active Scan completed')

# Report the results

print ('Hosts: {}'.format(', '.join(zap.core.hosts)))
print ('Alerts: ')
pprint (zap.core.alerts())