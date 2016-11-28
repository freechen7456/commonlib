#!/bin/sh
rsync -auP /srv rsync://16.156.209.27/docker > /tmp/rsync-docker.log 2> /tmp/rsync-docker-error.log
