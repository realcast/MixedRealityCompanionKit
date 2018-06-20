// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#pragma once

#include "DirectXHelper.h"
#include "CompositorShared.h"
#include <vector>
#include <memory>
#include <array>

#define MAX_QUEUE_SIZE 90

#define INVALID_TIMESTAMP -1

class HologramQueue;

typedef struct
{
    friend class HologramQueue;
public:
    LONGLONG timeStamp = INVALID_TIMESTAMP;
    float rotX, rotY, rotZ, rotW, posX, posY, posZ;

    int GetId() const { return m_id; }
private:
    int m_id;
} FrameMessage;

static std::array<FrameMessage, MAX_QUEUE_SIZE> m_holographicFrameQueue;
static BYTE* hologramQueueFrameData = new BYTE[HOLOGRAM_BUFSIZE];

class HologramQueue
{
public:
    HologramQueue();

    FrameMessage* GetNextFrame(LONGLONG timeStamp);
    FrameMessage* FindClosestFrame(LONGLONG timeStamp, LONGLONG frameOffset);

private:
    int m_nextIndex = 0;
};

