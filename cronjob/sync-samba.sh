#!/bin/sh
rsync -auP rsync://16.156.209.27/samba /home/samba > /tmp/rsync-samba.log 2> /tmp/rsync-samba-error.log
