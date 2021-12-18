package ru.renev.adventofcode2021

import kotlin.math.abs

class Day7 {
    data class Crab(val position: Int, val count: Int)

    data class Travel(val crab: Crab, val distance: Int)

    data class Result(val position: Int, val totalFuel: Long)

    fun part1(): Long {
        println("Day 7 part 1")

        return calculateMinimumFuel { it }
    }

    fun part2(): Long {
        println("Day 7 part 2")

        return calculateMinimumFuel { (it + 1) * it / 2 }
    }

    private fun calculateMinimumFuel(fuelCalculator: (distance: Int) -> Int): Long {
        val input = parseInput()

        val minPosition = input.minOf { it }
        val maxPosition = input.maxOf { it }

        val crabs = input.groupBy { it }
            .map { Crab(it.key, it.value.count()) }

        val result = (minPosition..maxPosition).map { position ->
            crabs.map {
                Travel(it, abs(it.position - position))
            }
                .fold(0L) { sum, pos ->
                    sum + fuelCalculator(pos.distance) * pos.crab.count
                }
                .let { Result(position, it) }
        }.maxByOrNull { it.totalFuel }!!

        return result.totalFuel
    }

    private fun parseInput(): List<Int> {
        return Day7Input.values.split(",").map { it.toInt() }
    }
}
